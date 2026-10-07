namespace _02_Application.DTOs.Dashboard
{
    public record CompanyDashboardResponseDto(
        string NomeExibicao,
        List<OrdemServicoResumoDto> OsAtivas,
        List<VagaResumoDto> MinhasVagas,
        int VagasAbertas,
        int CandidatosRecebidos,
        int LotesEmProducao,
        decimal InvestimentoTotal,
        List<CandidaturaResumoDto> CandidaturasRecentes,
        List<ConversaPreviewDto> ConversasRecentes);
}
