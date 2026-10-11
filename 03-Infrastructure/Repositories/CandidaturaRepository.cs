using _03_Infrastructure.Data;
using _04_Domain.Entities;
using _04_Domain.Enums;
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

        public async Task<_04_Domain.Entities.Vaga?> ObterVagaNaoExcluidaAsync(int vagaId)
        {
            return await _context.Vagas
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == vagaId && !v.IsDeleted);
        }

        public async Task<bool> ExisteCandidaturaAsync(int vagaId, int freelancerId)
        {
            return await _context.Candidaturas.AnyAsync(c =>
                c.VagaId == vagaId &&
                c.FreelancerId == freelancerId);
        }

        public async Task<Candidatura> CreateAsync(Candidatura candidatura)
        {
            _context.Candidaturas.Add(candidatura);
            await _context.SaveChangesAsync();
            return candidatura;
        }

        public async Task<ICollection<Candidatura>> GetByFreelancerAsync(int freelancerId)
        {
            return await _context.Candidaturas
                .AsNoTracking()
                .Include(c => c.Vaga)
                .Where(c => c.FreelancerId == freelancerId)
                .ToListAsync();
        }

        public async Task<ICollection<Candidatura>> GetFiltradasAsync(
            int? freelancerId,
            int? empresaId,
            int? vagaId,
            StatusCandidatura? status)
        {
            IQueryable<Candidatura> query = _context.Candidaturas
                .AsNoTracking()
                .Include(c => c.Vaga);

            if (freelancerId.HasValue)
                query = query.Where(c => c.FreelancerId == freelancerId.Value);

            if (empresaId.HasValue)
                query = query.Where(c => c.Vaga != null && c.Vaga.UsuarioId == empresaId.Value);

            if (vagaId.HasValue)
                query = query.Where(c => c.VagaId == vagaId.Value);

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            return await query.ToListAsync();
        }

        public async Task<Candidatura?> GetComVagaAsync(int vagaId, int freelancerId)
        {
            // ATENÇÃO: não alterar para '.AsNoTracking' — A entidade é mutada pelo CandidaturaService
            return await _context.Candidaturas
                .Include(c => c.Vaga)
                .FirstOrDefaultAsync(c =>
                    c.VagaId == vagaId &&
                    c.FreelancerId == freelancerId);
        }

        public async Task UpdateAsync(Candidatura candidatura)
        {
            _context.Candidaturas.Update(candidatura);
            await _context.SaveChangesAsync();
        }

        // Usados pelo Dashboard
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