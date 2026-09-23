namespace _02_Application.DTOs.Vaga
{
    public record RespostavagaJson(
        int Id,
        string Titulo,
        string Descricao,
        decimal Orcamento,
        DateTime DataPublicacao,
        bool Ativa,
        string Status,
        int UsuarioId
    );
}