using _03_Infrastructure.Data;
using _04_Domain.Entities;
using _04_Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _03_Infrastructure.Repositories
{
    public class CandidaturaRepository : ICandidaturaRepository
    {
        private readonly AppDbContext _context;

        public CandidaturaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Candidatura>> ListByFreelancerIdAsync(int freelancerId)
        {
            return await _context.Candidaturas
                .AsNoTracking()
                .Include(c => c.Vaga)
                .Where(c => c.FreelancerId == freelancerId && c.Vaga != null && !c.Vaga.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Candidatura>> ListByEmpresaIdAsync(int empresaUserId)
        {
            return await _context.Candidaturas
                .AsNoTracking()
                .Include(c => c.Vaga)
                .Where(c => c.Vaga != null && !c.Vaga.IsDeleted && c.Vaga.UsuarioId == empresaUserId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
