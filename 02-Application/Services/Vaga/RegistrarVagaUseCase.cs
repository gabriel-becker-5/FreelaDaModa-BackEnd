using _02_Application.DTOs.Vaga;
using _04_Domain.Interfaces;

namespace _02_Application.Services.Vaga
{
    public class RegistrarVagaUseCase
    {
        private readonly IVagaRepository _repository;

        public RegistrarVagaUseCase(IVagaRepository repository)
        {
            _repository = repository;
        }

        public async Task<RespostaVagaJson> Executar(RequisicaoRegistrarVagaJson requisicao)
        {
            // Mapeia os dados do DTO para a Entidade de Domínio informando o namespace completo
            var entidade = new _04_Domain.Entities.Vaga
            {
                Titulo = requisicao.Titulo,
                Descricao = requisicao.Descricao,
                Orcamento = requisicao.Orcamento,
                UsuarioId = requisicao.UsuarioId,
                DataPublicacao = DateTime.Now,
                Ativa = true
            };

            // Salva no repositório
            await _repository.AdicionarAsync(entidade);

            // Retorna o DTO de resposta preenchido
            return new RespostaVagaJson
            {
                Id = entidade.Id,
                Titulo = entidade.Titulo,
                Descricao = entidade.Descricao,
                Orcamento = entidade.Orcamento,
                DataPublicacao = entidade.DataPublicacao,
                Ativa = entidade.Ativa,
                UsuarioId = entidade.UsuarioId
            };
        }
    }
}