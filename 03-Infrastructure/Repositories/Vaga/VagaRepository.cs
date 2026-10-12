using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using _03_Infrastructure.Data;
using _04_Domain.Enums;

namespace _03_Infrastructure.Repositories.Vaga
{
    public class VagaRepository : IVagaRepository
    {
        private readonly AppDbContext _context;

        public VagaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(_04_Domain.Entities.Vaga vaga)
        {
            await _context.Vagas.AddAsync(vaga);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(_04_Domain.Entities.Vaga vaga)
        {
            _context.Vagas.Update(vaga);
            await _context.SaveChangesAsync();
        }

        public async Task<_04_Domain.Entities.Vaga?> ObterNaoExcluidaPorIdAsync(int id)
        {
            // ATENÇÃO: não alterar para '.AsNoTracking' — A entidade é mutada pelo VagaService
            return await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        }

        public async Task<IEnumerable<_04_Domain.Entities.Vaga>> ListarNaoExcluidasAsync(int? usuarioId, StatusVaga? status)
        {
            var query = _context.Vagas
                .AsNoTracking()
                .Where(v => !v.IsDeleted);

            if (usuarioId.HasValue)
                query = query.Where(v => v.UsuarioId == usuarioId.Value);

            if (status.HasValue)
                query = query.Where(v => v.Status == status.Value);

            return await query.ToListAsync();
        }

        // Usados pelo Dashboard
        // Filtra pelo campo Status (StatusVaga), não pelo legado Ativa (ver D7 do plano de dashboards).
        public async Task<List<_04_Domain.Entities.Vaga>> ListarAbertasRecentesAsync(int quantidade)
        {
            return await _context.Vagas
                .AsNoTracking()
                .Where(v => !v.IsDeleted && v.Status == StatusVaga.Aberta)
                .OrderByDescending(v => v.DataPublicacao)
                .Take(quantidade)
                .ToListAsync();
        }

        public async Task<List<_04_Domain.Entities.Vaga>> ListarPorEmpresaAsync(int empresaUserId)
        {
            return await _context.Vagas
                .AsNoTracking()
                .Where(v => !v.IsDeleted && v.UsuarioId == empresaUserId)
                .OrderByDescending(v => v.DataPublicacao)
                .ToListAsync();
        }
    }
}
