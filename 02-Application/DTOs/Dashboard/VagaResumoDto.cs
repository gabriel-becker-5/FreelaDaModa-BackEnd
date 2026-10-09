namespace _02_Application.DTOs.Dashboard
{
    public record VagaResumoDto(
        int Id,
        string Titulo,
        string Status,
        decimal Valor,
        DateTime DataPublicacao,
        int EmpresaId,
        string EmpresaNome);
}
