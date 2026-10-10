using _04_Domain.Enums;

namespace _02_Application.DTOs.Vaga
{
    public record VagaDto(
        int Id,
        string Titulo,
        string Descricao,
        Specialty Especialidade,
        ModalidadeVaga Modalidade,
        string Cidade,
        string Estado,
        decimal Orcamento,
        DateTime PrazoConclusao,
        DateTime DataPublicacao,
        DateTime? UpdatedAt,
        bool Ativa,
        StatusVaga Status,
        int UsuarioId
    );
}