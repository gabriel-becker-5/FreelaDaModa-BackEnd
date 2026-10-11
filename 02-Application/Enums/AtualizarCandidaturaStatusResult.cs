using _02_Application.DTOs.Candidaturas;

namespace _02_Application.Enums
{
    public enum AtualizarCandidaturaStatus
    {
        Success,
        NaoEncontrada,
        SemPermissao
    }

    public record AtualizarCandidaturaStatusResult(
        AtualizarCandidaturaStatus Status,
        CandidaturaDto? Candidatura = null);
}