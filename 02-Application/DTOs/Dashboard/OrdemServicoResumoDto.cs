namespace _02_Application.DTOs.Dashboard
{
    public record OrdemServicoResumoDto(
        int Id,
        string Titulo,
        string Status,
        string Categoria,
        string Modalidade,
        string Cidade,
        decimal Valor,
        DateTime Prazo,
        int? FreelancerId,
        string? FreelancerNome,
        int EmpresaId,
        string EmpresaNome);
}
