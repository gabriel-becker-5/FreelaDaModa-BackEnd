namespace _02_Application.DTOs.Vaga
{
    public record RequisicaoAtualizarVagaJson(
        string? Titulo,
        string? Descricao,
        decimal? Remuneracao,
        string? TipoContrato,
        string? Modalidade
    );
}