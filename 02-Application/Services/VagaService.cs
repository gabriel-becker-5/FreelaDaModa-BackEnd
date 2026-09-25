using _02_Application.DTOs.Vaga;
using _02_Application.Interfaces;
using _04_Domain.Interfaces;
using DominioVaga = _04_Domain.Entities.Vaga;

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
        var entidade = new DominioVaga
        {
            Titulo = requisicao.Titulo,
            Descricao = requisicao.Descricao,
            Orcamento = (decimal)requisicao.Salario,
            DataPublicacao = DateTime.Now,
            Ativa = true,
            UsuarioId = 1 // TODO: converter/buscar o Id do utilizador com base em emailUsuario, se necessário
        };

        await _vagaRepository.AdicionarAsync(entidade);
        return entidade;
    }

    public async Task<IEnumerable<object>> ObterTodasAsync()
    {
        return await _vagaRepository.ObterTodasAsync();
    }
}