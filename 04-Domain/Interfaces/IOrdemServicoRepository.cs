using _04_Domain.Entities;

namespace _04_Domain.Interfaces
{
    public interface IOrdemServicoRepository
    {
        Task<List<OrdemServico>> ListAllAsync();

        Task<OrdemServico?> GetByIdAsync(int id);

        Task<OrdemServico> CreateAsync(OrdemServico ordemServico);

        Task UpdateAsync(OrdemServico ordemServico);

        Task DeleteAsync(OrdemServico ordemServico);
    }
}