using _02_Application.DTOs.Candidaturas;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Services
{
    public class CandidaturaService : ICandidaturaService
    {
        private readonly ICandidaturaRepository _candidaturaRepository;

        public CandidaturaService(ICandidaturaRepository candidaturaRepository)
        {
            _candidaturaRepository = candidaturaRepository;
        }

        public async Task<CriarCandidaturaResult> CriarAsync(
            int freelancerId,
            int vagaId,
            string? mensagem)
        {
            var vaga = await _candidaturaRepository.ObterVagaNaoExcluidaAsync(vagaId);

            if (vaga == null)
            {
                return new CriarCandidaturaResult(CriarCandidaturaStatus.VagaNaoEncontrada);
            }

            if (vaga.Status != StatusVaga.Aberta)
            {
                return new CriarCandidaturaResult(CriarCandidaturaStatus.VagaNaoAberta);
            }

            bool candidaturaExistente =
                await _candidaturaRepository.ExisteCandidaturaAsync(vagaId, freelancerId);

            if (candidaturaExistente)
            {
                return new CriarCandidaturaResult(CriarCandidaturaStatus.JaCandidatado);
            }

            DateTime agora = DateTime.UtcNow;

            Candidatura novaCandidatura = new()
            {
                VagaId = vagaId,
                FreelancerId = freelancerId,
                UsuarioId = freelancerId,
                DataCandidatura = agora,
                Mensagem = mensagem,
                CreatedAt = agora,
                Status = StatusCandidatura.Pendente
            };

            await _candidaturaRepository.CreateAsync(novaCandidatura);

            return new CriarCandidaturaResult(
                CriarCandidaturaStatus.Success,
                ToDto(novaCandidatura));
        }

        public async Task<ICollection<MinhaCandidaturaDto>> GetMinhasAsync(int freelancerId)
        {
            ICollection<Candidatura> candidaturas =
                await _candidaturaRepository.GetByFreelancerAsync(freelancerId);

            return candidaturas
                .Select(c => new MinhaCandidaturaDto
                {
                    Id = c.Id,
                    VagaId = c.VagaId,
                    TituloVaga = c.Vaga != null ? c.Vaga.Titulo : "Vaga",
                    DataCandidatura = c.DataCandidatura,
                    CreatedAt = c.CreatedAt,
                    Status = c.Status,
                    Mensagem = c.Mensagem
                })
                .ToList();
        }

        public async Task<ICollection<CandidaturaResumoDto>> GetFiltradasAsync(
            int userId,
            bool isCompany,
            int? vagaId,
            StatusCandidatura? status)
        {
            // Restrição por role/ownership: cada usuário só vê as próprias candidaturas
            int? empresaId = isCompany ? userId : null;
            int? freelancerId = isCompany ? null : userId;

            ICollection<Candidatura> candidaturas =
                await _candidaturaRepository.GetFiltradasAsync(
                    freelancerId,
                    empresaId,
                    vagaId,
                    status);

            return candidaturas
                .Select(c => new CandidaturaResumoDto
                {
                    Id = c.Id,
                    VagaId = c.VagaId,
                    TituloVaga = c.Vaga != null ? c.Vaga.Titulo : "Vaga",
                    FreelancerId = c.FreelancerId,
                    DataCandidatura = c.DataCandidatura,
                    CreatedAt = c.CreatedAt,
                    Status = c.Status,
                    Mensagem = c.Mensagem
                })
                .ToList();
        }

        public async Task<AtualizarCandidaturaStatusResult> AtualizarStatusAsync(
            int empresaId,
            int vagaId,
            int freelancerId,
            StatusCandidatura status)
        {
            Candidatura? candidatura =
                await _candidaturaRepository.GetComVagaAsync(vagaId, freelancerId);

            if (candidatura == null || candidatura.Vaga == null)
            {
                return new AtualizarCandidaturaStatusResult(
                    AtualizarCandidaturaStatus.NaoEncontrada);
            }

            // Ownership: só a empresa dona da vaga pode alterar o status
            if (candidatura.Vaga.UsuarioId != empresaId)
            {
                return new AtualizarCandidaturaStatusResult(
                    AtualizarCandidaturaStatus.SemPermissao);
            }

            // Vaga excluída ou encerrada não aceita mais mudança de status
            if (candidatura.Vaga.IsDeleted || candidatura.Vaga.Status == StatusVaga.Encerrada)
            {
                return new AtualizarCandidaturaStatusResult(
                    AtualizarCandidaturaStatus.NaoEncontrada);
            }

            candidatura.Status = status;
            await _candidaturaRepository.UpdateAsync(candidatura);

            return new AtualizarCandidaturaStatusResult(
                AtualizarCandidaturaStatus.Success,
                ToDto(candidatura));
        }

        private static CandidaturaDto ToDto(Candidatura c)
        {
            return new CandidaturaDto
            {
                Id = c.Id,
                VagaId = c.VagaId,
                FreelancerId = c.FreelancerId,
                UsuarioId = c.UsuarioId,
                DataCandidatura = c.DataCandidatura,
                Mensagem = c.Mensagem,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            };
        }
    }
}