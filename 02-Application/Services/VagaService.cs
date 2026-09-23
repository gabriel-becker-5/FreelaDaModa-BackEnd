using _02_Application.DTOs.Vaga;
using _02_Application.Interfaces;
using _02_Application.Mappings;
using _03_Infrastructure.Repositories.Vaga;
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

    public async Task<RespostavagaJson> RegistrarAsync(RequisicaoRegistrarVagaJson requisicao, string emailUsuario)
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

    public async Task<IEnumerable<RespostavagaJson>> ObterTodasAsync()
    {
        var list = await _vagaRepository.ObterTodasAsync();
        return list.ParaRespostaJson();
    }

    public async Task<IEnumerable<RespostavagaJson>> ObterComFiltrosAsync(int? usuarioId, string? status)
    {
        var list = await _vagaRepository.ListarComFiltrosAsync(usuarioId, status);
        return list.ParaRespostaJson();
    }

    public async Task<RespostavagaJson?> ObterPorIdAsync(int id)
    {
        var entidade = await _vagaRepository.ObterPorIdAsync(id);
        return entidade?.ParaRespostaJson();
    }

    public async Task<RespostavagaJson?> AtualizarAsync(int id, RequisicaoAtualizarVagaJson requisicao)
    {
        var entidade = await _vagaRepository.ObterPorIdAsync(id);
        if (entidade == null) return null;

        if (!string.IsNullOrWhiteSpace(requisicao.Titulo))
            entidade.Titulo = requisicao.Titulo;

        if (!string.IsNullOrWhiteSpace(requisicao.Descricao))
            entidade.Descricao = requisicao.Descricao;

        await _vagaRepository.AtualizarAsync(entidade);
        return entidade.ParaRespostaJson();
    }

    public async Task<bool> AtualizarStatusAsync(int id, string status)
    {
        return await _vagaRepository.AtualizarStatusAsync(id, status);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        return await _vagaRepository.DeletarAsync(id);
    }
}