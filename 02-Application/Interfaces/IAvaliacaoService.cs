using _02_Application.DTOs;
using _04_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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