using _02_Application.DTOs.Vaga;
using _02_Application.Interfaces;
using _04_Domain.Interfaces; // Ajusta o namespace caso o IVagaRepository esteja noutro sítio (ex: _02_Application.Interfaces)

namespace _02_Application.Services;

public class VagaService : IVagaService
{
    private readonly IVagaRepository _vagaRepository;

    public VagaService(IVagaRepository vagaRepository)
    {
        _vagaRepository = vagaRepository;
    }

    public async Task<object> RegistrarAsync(RequisicaoRegistrarVagaJson requisicao, string emailUsuario)
    {
        // TODO: Mapear DTO para entidade de domínio e invocar _vagaRepository.AdicionarAsync(...)
        return new { mensagem = "Vaga criada com sucesso!", usuario = emailUsuario };
    }

    public async Task<IEnumerable<object>> ObterTodasAsync()
    {
        // TODO: Obter dados do repositório
        return new List<object>();
    }
}