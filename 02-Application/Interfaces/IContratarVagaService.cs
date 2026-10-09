using _02_Application.DTOs.Vaga;

namespace _02_Application.Interfaces
{
    public interface IContratarVagaService
    {
        Task<ContratarVagaResultadoDto> ContratarAsync(int vagaId, int usuarioLogadoId, int freelancerId);
    }
}
