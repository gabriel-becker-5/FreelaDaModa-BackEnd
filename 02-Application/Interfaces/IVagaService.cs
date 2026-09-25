using _02_Application.DTOs.Vaga;

namespace _02_Application.Interfaces;

public interface IVagaService
{
    Task<object> RegistrarAsync(RequisicaoRegistrarVagaJson requisicao, string emailUsuario);
    Task<IEnumerable<object>> ObterTodasAsync();
}