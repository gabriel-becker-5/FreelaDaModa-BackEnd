using _03_Infrastructure.Data;
using _04_Domain.Entities;
using _04_Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03_Infrastructure.Repositories
{
    public class AvaliacaoRepository : IAvaliacaoRepository
    {
        private readonly AppDbContext _context;

        public AvaliacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Avaliacao> CreateAsync(Avaliacao avaliacao)
        {
            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            return avaliacao;
        }

        public async Task<List<Avaliacao>> ListByOrdemServicoAsync(int ordemServicoId)
        {
            return await _context.Avaliacoes
                .Where(a => a.OrdemServicoId == ordemServicoId)
                .ToListAsync();
        }
    }
}

