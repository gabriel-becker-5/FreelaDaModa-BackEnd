using _02_Application.Enums;
using _03_Infrastructure.Data;
using _03_Infrastructure.Repositories;
using _03_Infrastructure.Services;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace _05_Tests
{
    public class ContratarVagaServiceTests : IDisposable
    {
        private const int IdEmpresa = 1;
        private const int IdOutraEmpresa = 2;
        private const int IdFreelancerEscolhido = 10;
        private const int IdFreelancerConcorrente = 11;
        private const int IdFreelancerExcluido = 12;
        private const int IdUsuarioSemRoleFreelancer = 13;
        private const int IdVaga = 100;

        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _options;

        public ContratarVagaServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using AppDbContext context = new(_options);
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        private AppDbContext NovoContexto() => new(_options);

        private static User CriarUsuario(int id, ICollection<Roles> roles, bool isDeleted = false)
        {
            return new User
            {
                Id = id,
                Email = $"usuario{id}@teste.com",
                LegalResponsibleFullName = "Responsável Teste",
                LegalResponsibleDocument = $"{id:D11}",
                PasswordHash = "hash",
                ContactNumber = "11999999999",
                PublicProfileDescription = "descrição",
                PostalCode = "01000000",
                Address = "Rua Teste",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Roles = roles,
                IsDeleted = isDeleted
            };
        }

        private async Task SeedBaseAsync(
            AppDbContext context,
            StatusVaga statusVaga = StatusVaga.Aberta,
            DateTime? prazoConclusao = null,
            bool vagaExcluida = false,
            bool incluirCandidaturaConcorrente = true,
            StatusCandidatura statusCandidaturaEscolhida = StatusCandidatura.Pendente)
        {
            context.Users.Add(CriarUsuario(IdEmpresa, new List<Roles> { Roles.Company }));
            context.Users.Add(CriarUsuario(IdOutraEmpresa, new List<Roles> { Roles.Company }));
            context.Users.Add(CriarUsuario(IdFreelancerEscolhido, new List<Roles> { Roles.Freelancer }));
            context.Users.Add(CriarUsuario(IdFreelancerConcorrente, new List<Roles> { Roles.Freelancer }));

            context.Vagas.Add(new Vaga
            {
                Id = IdVaga,
                Titulo = "Confecção de camisetas",
                Descricao = "Preciso de 100 camisetas",
                Especialidade = Specialty.Costura_Reta,
                Modalidade = ModalidadeVaga.Sob_Demanda,
                Cidade = "São Paulo",
                Estado = "SP",
                Orcamento = 1500.50m,
                PrazoConclusao = prazoConclusao ?? DateTime.UtcNow.AddDays(10),
                DataPublicacao = DateTime.UtcNow,
                Status = statusVaga,
                IsDeleted = vagaExcluida,
                UsuarioId = IdEmpresa
            });

            context.Candidaturas.Add(new Candidatura
            {
                VagaId = IdVaga,
                FreelancerId = IdFreelancerEscolhido,
                UsuarioId = IdFreelancerEscolhido,
                DataCandidatura = DateTime.UtcNow,
                Status = statusCandidaturaEscolhida
            });

            if (incluirCandidaturaConcorrente)
            {
                context.Candidaturas.Add(new Candidatura
                {
                    VagaId = IdVaga,
                    FreelancerId = IdFreelancerConcorrente,
                    UsuarioId = IdFreelancerConcorrente,
                    DataCandidatura = DateTime.UtcNow,
                    Status = StatusCandidatura.Pendente
                });
            }

            await context.SaveChangesAsync();
        }

        private static ContratarVagaService CriarServico(AppDbContext context)
        {
            return new ContratarVagaService(context, new UserRepository(context));
        }

        [Fact]
        public async Task ContratarAsync_DeveTerSucesso_EExecutarTodosOsEfeitos()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);

            await using AppDbContext contextoAcao = NovoContexto();
            ContratarVagaService servico = CriarServico(contextoAcao);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.Sucesso, resultado.Resultado);
            Assert.NotNull(resultado.OrdemServicoId);

            await using AppDbContext contextoVerificacao = NovoContexto();

            Vaga vaga = await contextoVerificacao.Vagas.SingleAsync(v => v.Id == IdVaga);
            Assert.Equal(StatusVaga.Encerrada, vaga.Status);
            Assert.NotNull(vaga.UpdatedAt);

            Candidatura escolhida = await contextoVerificacao.Candidaturas
                .SingleAsync(c => c.VagaId == IdVaga && c.FreelancerId == IdFreelancerEscolhido);
            Assert.Equal(StatusCandidatura.Aceita, escolhida.Status);

            Candidatura concorrente = await contextoVerificacao.Candidaturas
                .SingleAsync(c => c.VagaId == IdVaga && c.FreelancerId == IdFreelancerConcorrente);
            Assert.Equal(StatusCandidatura.Rejeitada, concorrente.Status);

            OrdemServico ordemServico = await contextoVerificacao.OrdensServico
                .SingleAsync(o => o.Id == resultado.OrdemServicoId);

            Assert.Equal(IdEmpresa, ordemServico.UserId);
            Assert.Equal(IdFreelancerEscolhido, ordemServico.FreelancerId);
            Assert.Equal("Em andamento", ordemServico.Status);
            Assert.Equal(vaga.Titulo, ordemServico.Titulo);
            Assert.Equal(vaga.Descricao, ordemServico.Descricao);
            Assert.Equal("Costura Reta", ordemServico.Categoria);
            Assert.Equal("Sob Demanda", ordemServico.Modalidade);
            Assert.Equal(vaga.Cidade, ordemServico.Cidade);
            Assert.Equal(vaga.Orcamento, ordemServico.Valor);
            Assert.Equal(vaga.PrazoConclusao, ordemServico.Prazo);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarCandidaturaJaAceita_QuandoOutraCandidaturaDaVagaJaEstaAceita()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);

            // Simula uma candidatura concorrente já Aceita por fora (ex.: PATCH antigo de status).
            // Correção de code review (#16): qualquer candidatura Aceita na vaga bloqueia uma nova
            // contratação, para não criar uma segunda OS.
            Candidatura concorrenteSetup = await contextoSetup.Candidaturas
                .SingleAsync(c => c.VagaId == IdVaga && c.FreelancerId == IdFreelancerConcorrente);
            concorrenteSetup.Status = StatusCandidatura.Aceita;
            await contextoSetup.SaveChangesAsync();

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.CandidaturaJaAceita, resultado.Resultado);

            await using AppDbContext contextoVerificacao = NovoContexto();

            Candidatura escolhida = await contextoVerificacao.Candidaturas
                .SingleAsync(c => c.VagaId == IdVaga && c.FreelancerId == IdFreelancerEscolhido);
            Assert.Equal(StatusCandidatura.Pendente, escolhida.Status);

            Vaga vaga = await contextoVerificacao.Vagas.SingleAsync(v => v.Id == IdVaga);
            Assert.Equal(StatusVaga.Aberta, vaga.Status);

            int totalOrdensServico = await contextoVerificacao.OrdensServico.CountAsync();
            Assert.Equal(0, totalOrdensServico);
        }

        [Fact]
        public async Task ContratarAsync_QuandoVagaEhReabertaAposContratacao_DeveRetornarCandidaturaJaAceita_ENaoCriarSegundaOs()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);

            await using AppDbContext contexto1 = NovoContexto();
            ContratarVagaService servico1 = CriarServico(contexto1);
            var primeiraChamada = await servico1.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);
            Assert.Equal(ContratarVagaResultado.Sucesso, primeiraChamada.Resultado);

            // Reabre a vaga "por fora" deste endpoint (ex.: PATCH de status em VagasController),
            // deixando a candidatura Aceita da contratação anterior intacta.
            await using AppDbContext contextoReabertura = NovoContexto();
            Vaga vagaReaberta = await contextoReabertura.Vagas.SingleAsync(v => v.Id == IdVaga);
            vagaReaberta.Status = StatusVaga.Aberta;
            await contextoReabertura.SaveChangesAsync();

            await using AppDbContext contexto2 = NovoContexto();
            ContratarVagaService servico2 = CriarServico(contexto2);
            var segundaChamada = await servico2.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerConcorrente);

            Assert.Equal(ContratarVagaResultado.CandidaturaJaAceita, segundaChamada.Resultado);

            await using AppDbContext contextoVerificacao = NovoContexto();
            int totalOrdensServico = await contextoVerificacao.OrdensServico.CountAsync();
            Assert.Equal(1, totalOrdensServico);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarUsuarioInativo_QuandoUsuarioLogadoEstaExcluido()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            contextoSetup.Users.Add(CriarUsuario(IdEmpresa, new List<Roles> { Roles.Company }, isDeleted: true));
            await contextoSetup.SaveChangesAsync();

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.UsuarioInativo, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarVagaNaoEncontrada_QuandoVagaNaoExiste()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            contextoSetup.Users.Add(CriarUsuario(IdEmpresa, new List<Roles> { Roles.Company }));
            await contextoSetup.SaveChangesAsync();

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(9999, IdEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.VagaNaoEncontrada, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarVagaNaoEncontrada_QuandoVagaEstaExcluida()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup, vagaExcluida: true);

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.VagaNaoEncontrada, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarNaoAutorizado_QuandoUsuarioNaoEhDono()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdOutraEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.NaoAutorizado, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarNaoAutorizado_SemVazarQueVagaEstaEncerrada()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup, statusVaga: StatusVaga.Encerrada);

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdOutraEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.NaoAutorizado, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarVagaEncerrada_QuandoVagaJaEstaEncerrada()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup, statusVaga: StatusVaga.Encerrada);

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.VagaEncerrada, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarFreelancerInvalido_QuandoFreelancerNaoExiste()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, 999999);

            Assert.Equal(ContratarVagaResultado.FreelancerInvalido, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarFreelancerInvalido_QuandoFreelancerEstaExcluido()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);
            contextoSetup.Users.Add(CriarUsuario(IdFreelancerExcluido, new List<Roles> { Roles.Freelancer }, isDeleted: true));
            contextoSetup.Candidaturas.Add(new Candidatura
            {
                VagaId = IdVaga,
                FreelancerId = IdFreelancerExcluido,
                UsuarioId = IdFreelancerExcluido,
                DataCandidatura = DateTime.UtcNow,
                Status = StatusCandidatura.Pendente
            });
            await contextoSetup.SaveChangesAsync();

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerExcluido);

            Assert.Equal(ContratarVagaResultado.FreelancerInvalido, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarFreelancerInvalido_QuandoUsuarioNaoTemRoleFreelancer()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);
            contextoSetup.Users.Add(CriarUsuario(IdUsuarioSemRoleFreelancer, new List<Roles> { Roles.Company }));
            contextoSetup.Candidaturas.Add(new Candidatura
            {
                VagaId = IdVaga,
                FreelancerId = IdUsuarioSemRoleFreelancer,
                UsuarioId = IdUsuarioSemRoleFreelancer,
                DataCandidatura = DateTime.UtcNow,
                Status = StatusCandidatura.Pendente
            });
            await contextoSetup.SaveChangesAsync();

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdUsuarioSemRoleFreelancer);

            Assert.Equal(ContratarVagaResultado.FreelancerInvalido, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarCandidaturaNaoEncontrada_QuandoFreelancerNaoSeCandidatou()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup, incluirCandidaturaConcorrente: false);

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerConcorrente);

            Assert.Equal(ContratarVagaResultado.CandidaturaNaoEncontrada, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarCandidaturaRejeitada_QuandoCandidaturaJaFoiRejeitada()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup, statusCandidaturaEscolhida: StatusCandidatura.Rejeitada);

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.CandidaturaRejeitada, resultado.Resultado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornarPrazoNoPassado_QuandoPrazoDaVagaJaPassou()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup, prazoConclusao: DateTime.UtcNow.AddDays(-1));

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            var resultado = await servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);

            Assert.Equal(ContratarVagaResultado.PrazoNoPassado, resultado.Resultado);

            await using AppDbContext contextoVerificacao = NovoContexto();
            Vaga vaga = await contextoVerificacao.Vagas.SingleAsync(v => v.Id == IdVaga);
            Assert.Equal(StatusVaga.Aberta, vaga.Status);
        }

        [Fact]
        public async Task ContratarAsync_SegundaChamada_DeveRetornarVagaEncerrada_ENaoCriarSegundaOrdemServico()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);

            await using AppDbContext contexto1 = NovoContexto();
            ContratarVagaService servico1 = CriarServico(contexto1);
            var primeiraChamada = await servico1.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido);
            Assert.Equal(ContratarVagaResultado.Sucesso, primeiraChamada.Resultado);

            await using AppDbContext contexto2 = NovoContexto();
            ContratarVagaService servico2 = CriarServico(contexto2);
            var segundaChamada = await servico2.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerConcorrente);

            Assert.Equal(ContratarVagaResultado.VagaEncerrada, segundaChamada.Resultado);

            await using AppDbContext contextoVerificacao = NovoContexto();
            int totalOrdensServico = await contextoVerificacao.OrdensServico.CountAsync(o => o.FreelancerId == IdFreelancerEscolhido || o.FreelancerId == IdFreelancerConcorrente);
            Assert.Equal(1, totalOrdensServico);

            Candidatura concorrente = await contextoVerificacao.Candidaturas
                .SingleAsync(c => c.VagaId == IdVaga && c.FreelancerId == IdFreelancerConcorrente);
            Assert.Equal(StatusCandidatura.Rejeitada, concorrente.Status);
        }

        [Fact]
        public async Task ContratarAsync_DeveDesfazerTudo_QuandoUmaEtapaDaTransacaoFalha()
        {
            await using AppDbContext contextoSetup = NovoContexto();
            await SeedBaseAsync(contextoSetup);

            // Remove a tabela de Ordens de Serviço para forçar uma falha no SaveChanges final
            // (após a reivindicação atômica da vaga já ter sido executada), validando que a
            // transação desfaz também a reivindicação da vaga e as rejeições de candidatura.
            await contextoSetup.Database.ExecuteSqlRawAsync("DROP TABLE OrdensServico");

            await using AppDbContext contexto = NovoContexto();
            ContratarVagaService servico = CriarServico(contexto);

            await Assert.ThrowsAnyAsync<DbUpdateException>(
                () => servico.ContratarAsync(IdVaga, IdEmpresa, IdFreelancerEscolhido));

            await using AppDbContext contextoVerificacao = NovoContexto();
            Vaga vaga = await contextoVerificacao.Vagas.SingleAsync(v => v.Id == IdVaga);
            Assert.Equal(StatusVaga.Aberta, vaga.Status);

            Candidatura escolhida = await contextoVerificacao.Candidaturas
                .SingleAsync(c => c.VagaId == IdVaga && c.FreelancerId == IdFreelancerEscolhido);
            Assert.Equal(StatusCandidatura.Pendente, escolhida.Status);

            Candidatura concorrente = await contextoVerificacao.Candidaturas
                .SingleAsync(c => c.VagaId == IdVaga && c.FreelancerId == IdFreelancerConcorrente);
            Assert.Equal(StatusCandidatura.Pendente, concorrente.Status);
        }
    }
}
