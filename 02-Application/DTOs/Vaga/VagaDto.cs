using _04_Domain.Enums;

namespace _02_Application.DTOs.Vaga
{
    public record VagaDto(
        int Id,
        string Titulo,
        string Descricao,
        decimal Orcamento,
        DateTime DataPublicacao,
        bool Ativa,
        StatusVaga Status,
        int UsuarioId,
        DateTime? UpdatedAt
    );
}