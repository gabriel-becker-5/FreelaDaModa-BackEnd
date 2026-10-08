using System.Collections.Generic;
using System.Threading.Tasks;
using _04_Domain.Entities;
using _04_Domain.Enums;

namespace _03_Infrastructure.Repositories.Vaga;

public interface IVagaRepository
{
    Task AdicionarAsync(_04_Domain.Entities.Vaga vaga);
    Task AtualizarAsync(_04_Domain.Entities.Vaga vaga);
    Task<_04_Domain.Entities.Vaga?> ObterPorIdAsync(int id);
    Task<IEnumerable<_04_Domain.Entities.Vaga>> ObterPorUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<_04_Domain.Entities.Vaga>> ObterTodasAsync();
    Task<IEnumerable<_04_Domain.Entities.Vaga>> ListarComFiltrosAsync(int? usuarioId, string? status);
    Task<bool> AtualizarStatusAsync(int id, string status);
    Task<bool> DeletarAsync(int id);

    // Busca uma vaga que não foi excluída (IsDeleted = false)
    // ATENÇÃO: retorna a entidade rastreada, pois é alterada pelo VagaService
    Task<_04_Domain.Entities.Vaga?> ObterNaoExcluidaPorIdAsync(int id);

    // Lista as vagas não excluídas, filtrando por dono e por StatusVaga
    Task<IEnumerable<_04_Domain.Entities.Vaga>> ListarNaoExcluidasAsync(int? usuarioId, StatusVaga? status);

    // Usados pelo Dashboard
    Task<List<_04_Domain.Entities.Vaga>> ListarAbertasRecentesAsync(int quantidade);
    Task<List<_04_Domain.Entities.Vaga>> ListarPorEmpresaAsync(int empresaUserId);
}