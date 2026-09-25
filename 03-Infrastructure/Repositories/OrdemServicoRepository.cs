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
    public class OrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly AppDbContext _context;

        public OrdemServicoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrdemServico>> ListAllAsync()
        {
            return await _context.OrdensServico
                .Where(o => !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<OrdemServico?> GetByIdAsync(int id)
        {
            return await _context.OrdensServico
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        public async Task<OrdemServico> CreateAsync(OrdemServico ordemServico)
        {
            _context.OrdensServico.Add(ordemServico);

            await _context.SaveChangesAsync();

            return ordemServico;
        }

        public async Task UpdateAsync(OrdemServico ordemServico)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrdemServico ordemServico)
        {
            ordemServico.IsDeleted = true;

            _context.OrdensServico.Update(ordemServico);
            await _context.SaveChangesAsync();
        }

    }
}

