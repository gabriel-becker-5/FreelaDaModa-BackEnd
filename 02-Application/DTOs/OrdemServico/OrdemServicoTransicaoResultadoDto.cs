using _02_Application.Enums;

namespace _02_Application.DTOs.OrdemServico
{
    public record OrdemServicoTransicaoResultadoDto(
        OrdemServicoTransicaoResultado Resultado,
        string Mensagem,
        int? OrdemServicoId
    );
}
