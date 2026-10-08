namespace _02_Application.DTOs.Dashboard
{
    public record VagaResumoDto(
        int Id,
        string Titulo,
        string Status,
        decimal Valor,
        DateTime CreatedAt,
        int EmpresaId,
        string EmpresaNome);
}
