using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Interfaces;

namespace _02_Application.Services
{
    public class MensagemService : IMensagemService
    {
        private readonly IMensagemRepository _mensagemRepository;

        public MensagemService(IMensagemRepository mensagemRepository)
        {
            _mensagemRepository = mensagemRepository;
        }

        public async Task<List<Mensagem>> ListarAsync()
        {
            return await _mensagemRepository.ListarAsync();
        }

        public async Task<Mensagem?> BuscarPorIdAsync(int id)
        {
            return await _mensagemRepository.BuscarPorIdAsync(id);
        }

        public async Task<Mensagem> CriarAsync(MensagemDto dto)
        {
            Mensagem mensagem = new()
            {
                RemetenteId = dto.RemetenteId,
                DestinatarioId = dto.DestinatarioId,
                Conteudo = dto.Conteudo,
                DataEnvio = DateTime.Now
            };

            return await _mensagemRepository.CriarAsync(mensagem);
        }
    }
}

