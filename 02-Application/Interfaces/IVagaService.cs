using _02_Application.DTOs.Vaga;

namespace _02_Application.Interfaces;

public interface IVagaService
{
    Task<RespostavagaJson> RegistrarAsync(RequisicaoRegistrarVagaJson requisicao, string emailUsuario);
    Task<IEnumerable<RespostavagaJson>> ObterTodasAsync();
    Task<IEnumerable<RespostavagaJson>> ObterComFiltrosAsync(int? usuarioId, string? status);
    Task<RespostavagaJson?> ObterPorIdAsync(int id);
    Task<RespostavagaJson?> AtualizarAsync(int id, RequisicaoAtualizarVagaJson requisicao);
    Task<bool> AtualizarStatusAsync(int id, string status);
    Task<bool> DeletarAsync(int id);
}