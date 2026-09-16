using _04_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_Domain.Interfaces
{
    public interface IOrdemServicoRepository
    {
        Task<List<OrdemServico>> ListAllAsync();

        Task<OrdemServico?> GetByIdAsync(int id);

        Task<OrdemServico> CreateAsync(OrdemServico ordemServico);

        Task UpdateAsync(OrdemServico ordemServico);
    }
}
