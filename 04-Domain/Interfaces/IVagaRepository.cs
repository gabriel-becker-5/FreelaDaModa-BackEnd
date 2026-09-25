using System.Collections.Generic;
using System.Threading.Tasks;
using _04_Domain.Entities;

namespace _04_Domain.Interfaces
{
    public interface IVagaRepository
    {
        Task AdicionarAsync(Vaga vaga);
        Task AtualizarAsync(Vaga vaga);
        Task<Vaga?> ObterPorIdAsync(int id);
        Task<IEnumerable<Vaga>> ObterPorUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Vaga>> ObterTodasAsync();
    }
}