using _02_Application.DTOs.Vaga;
using _02_Application.Mappings;
using _03_Infrastructure.Repositories.Vaga;
using _04_Domain.Interfaces;
using DominioVaga = _04_Domain.Entities.Vaga;

namespace _02_Application.Services.Vaga;

public class RegistrarVagaUseCase
{
    private readonly IVagaRepository _vagaRepository;

    public RegistrarVagaUseCase(IVagaRepository vagaRepository)
    {
        _vagaRepository = vagaRepository;
    }

    public async Task<RespostavagaJson> ExecutarAsync(RequisicaoRegistrarVagaJson requisicao, string emailUsuario)
    {
        var entidade = new DominioVaga
        {
            Titulo = requisicao.Titulo,
            Descricao = requisicao.Descricao,
            Orcamento = Convert.ToDecimal(requisicao.Salario),
            DataPublicacao = DateTime.Now,
            Ativa = true,
            UsuarioId = 1
        };

        await _vagaRepository.AdicionarAsync(entidade);
        return entidade.ParaRespostaJson();
    }
}