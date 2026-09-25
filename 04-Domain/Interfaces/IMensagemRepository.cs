using _04_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_Domain.Interfaces
{
    public interface IMensagemRepository
    {
        Task<List<Mensagem>> ListarAsync();

        Task<Mensagem?> BuscarPorIdAsync(int id);

        Task<Mensagem> CriarAsync(Mensagem mensagem);
    }
}
