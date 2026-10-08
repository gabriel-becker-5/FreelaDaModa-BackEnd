using _02_Application.DTOs.Candidaturas;

namespace _02_Application.Enums
{
    public enum CriarCandidaturaStatus
    {
        Success,
        VagaNaoEncontrada,
        JaCandidatado
    }

    public record CriarCandidaturaResult(
        CriarCandidaturaStatus Status,
        CandidaturaDto? Candidatura = null);
}