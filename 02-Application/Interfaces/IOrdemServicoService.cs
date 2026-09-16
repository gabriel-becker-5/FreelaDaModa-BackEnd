using _02_Application.DTOs;
using _04_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_Application.Interfaces
{
    public interface IOrdemServicoService
    {
        Task<List<OrdemServico>> ListAllAsync();

        Task<OrdemServico?> GetByIdAsync(int id);

        Task<OrdemServico> CreateAsync(OrdemServicoDto dto);

        Task<bool> UpdateAsync(OrdemServicoDto dto);
    }
}
