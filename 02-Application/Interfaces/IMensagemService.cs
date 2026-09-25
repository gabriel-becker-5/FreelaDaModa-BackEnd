using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _02_Application.DTOs;
using _04_Domain.Entities;
using System.Threading.Tasks;

namespace _02_Application.Interfaces
{
    public interface IMensagemService
    {
        Task<List<Mensagem>> ListarAsync();

        Task<Mensagem?> BuscarPorIdAsync(int id);

        Task<Mensagem> CriarAsync(MensagemDto dto);
    }
}
