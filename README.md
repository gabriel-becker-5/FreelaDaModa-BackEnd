# Projeto Integrador Entra 21 - Turma C# - SENAC Blumenau
# Equipe: Karina Vicente, Lucas Baer, Charles Júnior e Gabriel Becker.

# Back-End Freela da Moda
API do marketplace que conecta confecções (empresas) e profissionais têxteis (freelancers).

- **Stack**: C# / .NET 8, ASP.NET Core Web API, EF Core com **Pomelo MySQL**, autenticação **JWT** e API Versioning.
- **Camadas**: `01-Presentation` (controllers/configuração), `02-Application` (serviços/DTOs), `03-Infrastructure` (dados/migrations/seed), `04-Domain` (entidades/interfaces), `05-Tests`.
- **Front-end**: repositório separado, consome esta API em `http://localhost:5104/api/v1`.

## Pré-requisitos
- SDK .NET 8
- MySQL (ex.: MySQL 8 local, porta 3306) — o banco `freeladamoda` é criado pelas migrations na subida

## Configuração (User Secrets)
Os `appsettings` contêm apenas placeholders. Os valores reais devem ficar em **User Secrets** (fora do versionamento):

```bash
dotnet user-secrets init --project 01-Presentation
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=freeladamoda;User=SEU_USUARIO;Password=SUA_SENHA" --project 01-Presentation
dotnet user-secrets set "Jwt:Key" "SUA_CHAVE_JWT" --project 01-Presentation
dotnet user-secrets set "AdminSeed:Email" "SEU_EMAIL_ADMIN" --project 01-Presentation
dotnet user-secrets set "AdminSeed:Password" "SUA_SENHA_ADMIN" --project 01-Presentation
```

Ao iniciar o pipeline, a API roda automaticamente as **migrations** e o **seed do administrador** (papel `Admin`).

## Como rodar
```bash
dotnet run --project 01-Presentation
```

- Perfil padrão `http`: **`http://localhost:5104`**
- Swagger em `/swagger` (ambiente de Development)

Build e testes:

```bash
dotnet build FreelaDaModa.sln
dotnet test FreelaDaModa.sln
```

## Papéis
- `Admin` — criado pelo seed (`AdminSeed`), gerencia usuários e roles
- `Company` e `Freelancer` — criados pelo cadastro (telas 05/06 do front ou `POST /User/cadastrar/...`)

## Rotas da demo
Todas sob `/api/v1`. Autenticação via `Authorization: Bearer <token>` (login é anônimo; listagem de vagas e perfis públicos também).

| Recurso | Rotas principais |
|---|---|
| Login | `POST /Authentication/login` |
| Cadastro e conta | `POST /User/cadastrar/freelancer`, `POST /User/cadastrar/empresa`, `GET /User/perfil/{freelancer\|empresa}`, `PATCH /User/atualizarcadastro/{freelancer\|empresa}`, `DELETE /User/deletarcadastro` |
| Foto de perfil | `PUT /User/perfil/imagem`, `GET /User/{userId}/imagem-perfil`, `DELETE /User/perfil/imagem` |
| Campos de cadastro | `GET /FreelancerFields/listar/camposDoFreelancer` |
| Catálogo e perfis públicos | `GET /Freelancer/listarFreelancers`, `GET /Freelancer/exibirPerfilPublicoFreelancer`, `GET /Company/exibirPerfilPublicoEmpresa` |
| Vagas | `POST /vagas`, `GET /vagas`, `GET /vagas/{id}`, `PATCH /vagas/{id}`, `PATCH /vagas/{id}/status`, `DELETE /vagas/{id}` (soft delete) |
| Candidaturas | `POST /User/candidaturas`, `GET /User/candidaturas/minhas`, `GET /User/candidaturas`, `PATCH /User/candidaturas/status` |
| Contratação | `POST /vagas/{vagaId}/contratar` |
| Ordens de serviço | `GET /OrdemServico/listar`, `GET /OrdemServico/detalhe/{id}`, `POST /OrdemServico/cadastrar`, `PUT /OrdemServico/editar`, `DELETE /OrdemServico/excluir/{id}`, `POST /OrdemServico/{id}/concluir`, `POST /OrdemServico/{id}/cancelar` |
| Chat | `GET /Mensagem/listar`, `GET /Mensagem/detalhe/{id}`, `POST /Mensagem/enviar` |
| Avaliações | `POST /Avaliacao/enviar`, `GET /Avaliacao/listar/{ordemServicoId}`, `PUT /Avaliacao/editar/{id}`, `DELETE /Avaliacao/excluir/{id}` |
| Dashboards | `GET /Dashboard/freelancer`, `GET /Dashboard/empresa` |
| Admin | `GET /Admin/listaUsuarios`, `GET /Admin/pesquisaUsuarioPorId`, `DELETE /Admin/deletaUsuario`, `GET /Admin/listaRoles`, `POST /Admin/concederAcesso`, `DELETE /Admin/revogarAcesso`, `DELETE /Admin/revogarTodosAcessos`, `GET /Admin/exibirAcessosUsuario` |

Detalhes de contratos e parâmetros: Swagger (`/swagger`).

## Integração com o front-end
O front aponta para esta API em `js/config.js` (`API_BASE = 'http://localhost:5104/api/v1'`). Para rodar o produto completo, suba esta API antes do front. Consulte o README do repositório do front-end para servir as telas.