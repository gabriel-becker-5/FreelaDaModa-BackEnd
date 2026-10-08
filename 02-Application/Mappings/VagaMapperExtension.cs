using _02_Application.DTOs.Vaga;
using DominioVaga = _04_Domain.Entities.Vaga;

namespace _02_Application.Mappings;

public static class VagaMapperExtension
{
    public static RespostavagaJson ParaRespostaJson(this DominioVaga entidade)
    {
        if (entidade == null) return null!;

        return new RespostavagaJson(
            entidade.Id,
            entidade.Titulo,
            entidade.Descricao,
            entidade.Especialidade,
            entidade.Modalidade,
            entidade.Cidade,
            entidade.Estado,
            entidade.Orcamento,
            entidade.PrazoConclusao,
            entidade.CreatedAt,
            entidade.UpdatedAt,
            entidade.Ativa,
            entidade.Status,
            entidade.UsuarioId
        );
    }

    public static IEnumerable<RespostavagaJson> ParaRespostaJson(this IEnumerable<DominioVaga> entidades)
    {
        return entidades?.Select(e => e.ParaRespostaJson()).ToList() ?? Enumerable.Empty<RespostavagaJson>();
    }
}