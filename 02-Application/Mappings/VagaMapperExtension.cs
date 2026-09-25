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
            entidade.Orcamento,
            entidade.DataPublicacao,
            entidade.Ativa,
            entidade.Ativa ? "aberta" : "pausada",
            entidade.UsuarioId
        );
    }

    public static IEnumerable<RespostavagaJson> ParaRespostaJson(this IEnumerable<DominioVaga> entidades)
    {
        return entidades?.Select(e => e.ParaRespostaJson()).ToList() ?? Enumerable.Empty<RespostavagaJson>();
    }
}