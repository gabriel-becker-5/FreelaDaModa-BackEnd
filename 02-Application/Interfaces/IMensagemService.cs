using _02_Application.DTOs;
using _04_Domain.Entities;

namespace _02_Application.Interfaces
{
    public interface IMensagemService
    {
        Task<List<Mensagem>> ListarAsync();

        Task<Mensagem?> BuscarPorIdAsync(int id);

        Task<Mensagem> CriarAsync(MensagemDto dto);
    }
}