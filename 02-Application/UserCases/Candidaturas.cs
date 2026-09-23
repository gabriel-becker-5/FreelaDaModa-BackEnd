using Microsoft.EntityFrameworkCore;
using _02_Application.DTOs.Candidatura;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Enums;

namespace _02_Application.UserCases
{
    public class Candidaturas
    {
        private readonly IAppDbContext _context;

        public Candidaturas(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Sucesso, string Mensagem, object? Dados)> CriarAsync(int freelancerId, CriarCandidaturaDto dto)
        {
            var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == dto.VagaId);
            if (vaga == null)
                return (false, "Vaga não encontrada.", null);

            if (!vaga.Ativa)
                return (false, "Não é possível candidatar-se a uma vaga inativa.", null);

            var jaCandidatou = await _context.Candidaturas
                .AnyAsync(c => c.VagaId == dto.VagaId && c.FreelancerId == freelancerId);

            if (jaCandidatou)
                return (false, "Já possui uma candidatura registada para esta vaga.", null);

            var novaCandidatura = new Candidatura
            {
                VagaId = dto.VagaId,
                FreelancerId = freelancerId,
                Mensagem = dto.Mensagem,
                Status = StatusCandidatura.Pendente
            };

            _context.Candidaturas.Add(novaCandidatura);

            var notificacao = new Notificacao
            {
                UsuarioId = vaga.UsuarioId,
                Titulo = "Nova candidatura recebida",
                Mensagem = $"Um freelancer candidatou-se à vaga #{vaga.Id}."
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();

            return (true, "Candidatura realizada com sucesso!", new { novaCandidatura.Id });
        }
    }
}