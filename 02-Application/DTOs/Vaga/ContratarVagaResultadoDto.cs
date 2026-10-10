using _02_Application.Enums;

namespace _02_Application.DTOs.Vaga
{
    public record ContratarVagaResultadoDto(
        ContratarVagaResultado Resultado,
        string Mensagem,
        int? OrdemServicoId
    );
}
