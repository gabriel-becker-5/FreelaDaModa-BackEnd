using _02_Application.DTOs;
using _04_Domain.Entities;

namespace _02_Application.Interfaces
{
    public interface IOrdemServicoService
    {
        Task<List<OrdemServico>> ListAllAsync();

        Task<OrdemServico?> GetByIdAsync(int id);

        Task<OrdemServico> CreateAsync(OrdemServicoDto dto);

        Task<bool> UpdateAsync(OrdemServicoDto dto);

        Task<bool> DeleteAsync(int id);
    }
}