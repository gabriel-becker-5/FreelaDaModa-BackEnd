using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using _03_Infrastructure.Data;
using _04_Domain.Entities;
using _04_Domain.Interfaces;

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

        public async Task<_04_Domain.Entities.Vaga?> ObterPorIdAsync(int id)
        {
            return await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<_04_Domain.Entities.Vaga>> ObterPorUsuarioIdAsync(int usuarioId)
        {
            return await _context.Vagas.Where(v => v.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<IEnumerable<_04_Domain.Entities.Vaga>> ObterTodasAsync()
        {
            return await _context.Vagas.ToListAsync();
        }

        public async Task<IEnumerable<_04_Domain.Entities.Vaga>> ListarComFiltrosAsync(int? usuarioId, string? status)
        {
            var query = _context.Vagas.AsQueryable();

            if (usuarioId.HasValue)
                query = query.Where(v => v.UsuarioId == usuarioId.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (bool.TryParse(status, out var bVal))
                {
                    query = query.Where(v => v.Ativa == bVal);
                }
                else
                {
                    var lower = status.ToLower();
                    if (lower == "aberta" || lower == "true")
                        query = query.Where(v => v.Ativa);
                    else if (lower == "pausada" || lower == "false")
                        query = query.Where(v => !v.Ativa);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<bool> AtualizarStatusAsync(int id, string status)
        {
            var vaga = await ObterPorIdAsync(id);
            if (vaga == null) return false;

            if (bool.TryParse(status, out var ativaVal))
            {
                vaga.Ativa = ativaVal;
            }
            else
            {
                var lower = status.ToLower();
                vaga.Ativa = (lower == "aberta" || lower == "true" || lower == "1");
            }

            _context.Vagas.Update(vaga);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var vaga = await ObterPorIdAsync(id);
            if (vaga == null) return false;

            _context.Vagas.Remove(vaga);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}