using _04_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_Domain.Interfaces
{
    public interface IAvaliacaoRepository
    {
        Task<Avaliacao> CreateAsync(Avaliacao avaliacao);
        Task<List<Avaliacao>> ListByOrdemServicoAsync(int ordemServicoId);
    }
}