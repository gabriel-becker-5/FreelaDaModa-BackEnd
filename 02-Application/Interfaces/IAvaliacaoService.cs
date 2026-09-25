using _02_Application.DTOs;
using _04_Domain.Entities;

namespace _02_Application.Interfaces
{
    public interface IAvaliacaoService
    {
        Task<Avaliacao> CreateAsync(AvaliacaoDto dto);

        Task<List<Avaliacao>> ListByOrdemServicoAsync(int ordemServicoId);

        Task<bool> UpdateAsync(int id, AvaliacaoDto dto);

        Task<bool> DeleteAsync(int id);
    }
}