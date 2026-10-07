using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _03_Infrastructure.Data;
using _04_Domain.Entities;
using _04_Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace _03_Infrastructure.Repositories
{
    public class MensagemRepository : IMensagemRepository
    {
        private readonly AppDbContext _context;

        public MensagemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mensagem>> ListarAsync(int usuarioId)
        {
            return await _context.Mensagens
                .Where(m => m.RemetenteId == usuarioId || m.DestinatarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Mensagem?> BuscarPorIdAsync(int id)
        {
            return await _context.Mensagens.FindAsync(id);
        }

        public async Task<Mensagem> CriarAsync(Mensagem mensagem)
        {
            _context.Mensagens.Add(mensagem);

            await _context.SaveChangesAsync();

            return mensagem;
        }

        public async Task<List<Mensagem>> ListByParticipanteAsync(int usuarioId)
        {
            return await _context.Mensagens
                .AsNoTracking()
                .Where(m => m.RemetenteId == usuarioId || m.DestinatarioId == usuarioId)
                .OrderByDescending(m => m.DataEnvio)
                .ToListAsync();
        }
    }
}
