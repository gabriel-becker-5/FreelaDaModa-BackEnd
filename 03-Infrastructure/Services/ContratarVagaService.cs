using _02_Application.DTOs.Vaga;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _02_Application.Validation;
using _03_Infrastructure.Data;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using _04_Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _03_Infrastructure.Services
{
    // Contrata um freelancer a partir de uma vaga (#16): aceita a candidatura, rejeita as demais,
    // encerra a vaga e cria a Ordem de Serviço em uma única transação atômica.
    public class ContratarVagaService : IContratarVagaService
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;

        public ContratarVagaService(AppDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<ContratarVagaResultadoDto> ContratarAsync(int vagaId, int usuarioLogadoId, int freelancerId)
        {
            // O filtro global de IsDeleted em User (AppDbContext) já exclui contas excluídas desta
            // consulta: usuário null cobre tanto "não existe" quanto "está excluído".
            User? usuarioLogado = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == usuarioLogadoId);

            if (usuarioLogado == null)
            {
                return new ContratarVagaResultadoDto(
                    ContratarVagaResultado.UsuarioInativo,
                    "Sua conta não está ativa ou não foi encontrada.",
                    null);
            }

            Vaga? vaga = await _context.Vagas
                .FirstOrDefaultAsync(v => v.Id == vagaId && !v.IsDeleted);

            if (vaga == null)
            {
                return new ContratarVagaResultadoDto(ContratarVagaResultado.VagaNaoEncontrada, "Vaga não encontrada.", null);
            }

            // Nunca revelar o estado da vaga para quem não é o dono.
            if (vaga.UsuarioId != usuarioLogadoId)
            {
                return new ContratarVagaResultadoDto(ContratarVagaResultado.NaoAutorizado, "Você não tem permissão para contratar nesta vaga.", null);
            }

            if (vaga.Status == StatusVaga.Encerrada)
            {
                return new ContratarVagaResultadoDto(ContratarVagaResultado.VagaEncerrada, "Esta vaga já foi encerrada.", null);
            }

            // Impede uma segunda OS quando a vaga é reaberta (ex.: PATCH de status) depois de já ter
            // sido contratada: a candidatura Aceita da contratação anterior continua existindo mesmo
            // que o status da vaga volte para Aberta.
            bool jaTemCandidaturaAceita = await _context.Candidaturas
                .AnyAsync(c => c.VagaId == vagaId && c.Status == StatusCandidatura.Aceita);

            if (jaTemCandidaturaAceita)
            {
                return new ContratarVagaResultadoDto(
                    ContratarVagaResultado.CandidaturaJaAceita,
                    "Esta vaga já tem uma candidatura aceita; não é possível contratar novamente.",
                    null);
            }

            string? erroFreelancer = await FreelancerValidator.ValidarAsync(_userRepository, freelancerId);
            if (erroFreelancer != null)
            {
                return new ContratarVagaResultadoDto(ContratarVagaResultado.FreelancerInvalido, erroFreelancer, null);
            }

            Candidatura? candidatura = await _context.Candidaturas
                .FirstOrDefaultAsync(c => c.VagaId == vagaId && c.FreelancerId == freelancerId);

            if (candidatura == null)
            {
                return new ContratarVagaResultadoDto(ContratarVagaResultado.CandidaturaNaoEncontrada, "Não existe candidatura deste freelancer para esta vaga.", null);
            }

            if (candidatura.Status == StatusCandidatura.Rejeitada)
            {
                return new ContratarVagaResultadoDto(ContratarVagaResultado.CandidaturaRejeitada, "Esta candidatura já foi rejeitada.", null);
            }

            if (vaga.PrazoConclusao < DateTime.UtcNow)
            {
                return new ContratarVagaResultadoDto(
                    ContratarVagaResultado.PrazoNoPassado,
                    "O prazo de conclusão da vaga já passou. Edite o prazo da vaga antes de contratar.",
                    null);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            // Reivindicação atômica da vaga: só segue quem conseguir mudar o status nesta UPDATE condicional.
            // Garante que duas chamadas simultâneas não contratem a mesma vaga duas vezes.
            int linhasAfetadas = await _context.Vagas
                .Where(v => v.Id == vagaId && !v.IsDeleted && v.Status != StatusVaga.Encerrada)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(v => v.Status, StatusVaga.Encerrada)
                    .SetProperty(v => v.UpdatedAt, DateTime.UtcNow));

            if (linhasAfetadas == 0)
            {
                await transaction.RollbackAsync();
                return new ContratarVagaResultadoDto(ContratarVagaResultado.VagaEncerrada, "Esta vaga já foi encerrada.", null);
            }

            candidatura.Status = StatusCandidatura.Aceita;

            List<Candidatura> outrasCandidaturas = await _context.Candidaturas
                .Where(c => c.VagaId == vagaId && c.FreelancerId != freelancerId && c.Status != StatusCandidatura.Rejeitada)
                .ToListAsync();

            foreach (Candidatura outra in outrasCandidaturas)
            {
                outra.Status = StatusCandidatura.Rejeitada;
            }

            OrdemServico ordemServico = new()
            {
                UserId = vaga.UsuarioId,
                FreelancerId = freelancerId,
                Titulo = vaga.Titulo,
                Descricao = vaga.Descricao,
                Categoria = vaga.Especialidade.ToString().Replace("_", " "),
                Modalidade = vaga.Modalidade.ToString().Replace("_", " "),
                Cidade = vaga.Cidade,
                Valor = vaga.Orcamento,
                Prazo = vaga.PrazoConclusao,
                Status = StatusOrdemServico.EmAndamento.ParaTexto()
            };

            _context.OrdensServico.Add(ordemServico);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ContratarVagaResultadoDto(ContratarVagaResultado.Sucesso, "Freelancer contratado com sucesso.", ordemServico.Id);
        }
    }
}
