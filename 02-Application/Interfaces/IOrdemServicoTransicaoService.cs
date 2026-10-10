using _02_Application.DTOs.OrdemServico;

namespace _02_Application.Interfaces
{
    public interface IOrdemServicoTransicaoService
    {
        Task<OrdemServicoTransicaoResultadoDto> ConcluirAsync(int ordemServicoId, int usuarioLogadoId);

        Task<OrdemServicoTransicaoResultadoDto> CancelarAsync(int ordemServicoId, int usuarioLogadoId);
    }
}
