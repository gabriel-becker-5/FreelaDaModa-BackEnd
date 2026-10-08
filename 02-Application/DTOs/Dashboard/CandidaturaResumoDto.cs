namespace _02_Application.DTOs.Dashboard
{
    public record CandidaturaResumoDto(
        int Id,
        int VagaId,
        string TituloVaga,
        int FreelancerId,
        string FreelancerNome,
        string Status,
        DateTime DataCandidatura);
}
