using _04_Domain.Entities;

namespace _04_Domain.Interfaces
{
    public interface IAvaliacaoRepository
    {
        Task<Avaliacao> CreateAsync(Avaliacao avaliacao);
        Task<List<Avaliacao>> ListByOrdemServicoAsync(int ordemServicoId);

        Task<Avaliacao?> GetByIdAsync(int id);
        Task UpdateAsync(Avaliacao avaliacao);
        Task DeleteAsync(Avaliacao avaliacao);
    }
}