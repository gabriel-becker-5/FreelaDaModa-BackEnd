namespace _02_Application.DTOs.Dashboard
{
    public record FreelancerDashboardResponseDto(
        string NomeExibicao,
        decimal? MediaAvaliacao,
        int TotalAvaliacoes,
        List<VagaResumoDto> VagasRecomendadas,
        int ProducoesAtivas,
        int CandidaturasEnviadas,
        decimal FaturamentoTotal,
        List<OrdemServicoResumoDto> Producoes,
        List<ConversaPreviewDto> ConversasRecentes);
}
