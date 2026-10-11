using _02_Application.DTOs.Vaga;
using _02_Application.Enums;
using _04_Domain.Enums;

namespace _02_Application.Interfaces;

public interface IVagaService
{
    Task<RespostavagaJson> RegistrarAsync(RequisicaoRegistrarVagaJson requisicao, int usuarioId);
    Task<IEnumerable<RespostavagaJson>> ObterTodasAsync();
    Task<IEnumerable<RespostavagaJson>> ObterComFiltrosAsync(int? usuarioId, string? status);
    Task<RespostavagaJson?> ObterPorIdAsync(int id);
    Task<RespostavagaJson?> AtualizarAsync(int id, RequisicaoAtualizarVagaJson requisicao);
    Task<bool> AtualizarStatusAsync(int id, string status);
    Task<bool> DeletarAsync(int id);

    // Lista as vagas não excluídas, filtrando por dono e por StatusVaga
    Task<IEnumerable<VagaDto>> ListarAsync(int? usuarioId, StatusVaga? status);

    // Retorna null quando a vaga não existe ou foi excluída
    Task<VagaDto?> BuscarPorIdAsync(int id);

    // Só a empresa dona da vaga pode editar, alterar o status ou excluir (soft delete)
    Task<VagaOperacaoResult> EditarAsync(string emailUsuario, int id, UpdateVagaDto dto);
    Task<VagaOperacaoResult> AlterarStatusAsync(string emailUsuario, int id, StatusVaga status);
    Task<VagaOperacaoResult> ExcluirAsync(string emailUsuario, int id);
}