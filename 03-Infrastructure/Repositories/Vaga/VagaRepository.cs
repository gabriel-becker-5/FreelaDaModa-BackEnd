using _03_Infrastructure.Data;
using _04_Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _03_Infrastructure.Repositories.Vaga
{
    public class VagaRepository : IVagaRepository
    {
        private readonly AppDbContext _context; // Ajustado para o DbContext padrão do projeto

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
    }
}