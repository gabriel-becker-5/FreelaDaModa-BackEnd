using System.Collections.Generic;
using System.Threading.Tasks;
using _04_Domain.Entities;

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
}