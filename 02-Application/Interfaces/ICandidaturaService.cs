using _02_Application.DTOs.Candidaturas;
using _02_Application.Enums;
using _04_Domain.Enums;

namespace _02_Application.Interfaces
{
    public interface ICandidaturaService
    {
        Task<CriarCandidaturaResult> CriarAsync(
            int freelancerId,
            int vagaId,
            string? mensagem);

        Task<ICollection<MinhaCandidaturaDto>> GetMinhasAsync(int freelancerId);

        // Company vê as candidaturas das suas vagas; freelancer vê as próprias
        Task<ICollection<CandidaturaResumoDto>> GetFiltradasAsync(
            int userId,
            bool isCompany,
            int? vagaId,
            StatusCandidatura? status);

        // Só a empresa dona de uma vaga ativa pode alterar o status
        Task<AtualizarCandidaturaStatusResult> AtualizarStatusAsync(
            int empresaId,
            int vagaId,
            int freelancerId,
            StatusCandidatura status);
    }
}