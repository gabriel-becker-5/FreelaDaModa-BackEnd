using _04_Domain.Entities;

namespace _04_Domain.Interfaces
{
    public interface IMensagemRepository
    {
        Task<List<Mensagem>> ListarAsync();

        Task<Mensagem?> BuscarPorIdAsync(int id);

        Task<Mensagem> CriarAsync(Mensagem mensagem);
    }
}