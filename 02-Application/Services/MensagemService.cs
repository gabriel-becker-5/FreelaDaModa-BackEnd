using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Interfaces;

namespace _02_Application.Services
{
    public class MensagemService : IMensagemService
    {
        private readonly IMensagemRepository _mensagemRepository;
        private readonly IUserRepository _userRepository;

        public MensagemService(
            IMensagemRepository mensagemRepository,
            IUserRepository userRepository)
        {
            _mensagemRepository = mensagemRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Mensagem>> ListarAsync(int usuarioId)
        {
            return await _mensagemRepository.ListarAsync(usuarioId);
        }

        public async Task<Mensagem?> BuscarPorIdAsync(int id)
        {
            return await _mensagemRepository.BuscarPorIdAsync(id);
        }

        public async Task<Mensagem> CriarAsync(MensagemDto dto, int remetenteId)
        {
            if (dto.DestinatarioId <= 0)
            {
                throw new ArgumentException("Destinatário inválido ou inativo.");
            }

            User? destinatario =
                await _userRepository.GetUserByIdAsync(dto.DestinatarioId);

            if (destinatario == null || destinatario.IsDeleted)
            {
                throw new ArgumentException("Destinatário inválido ou inativo.");
            }

            Mensagem mensagem = new()
            {
                RemetenteId = remetenteId,
                DestinatarioId = dto.DestinatarioId,
                Conteudo = dto.Conteudo,
                DataEnvio = DateTime.UtcNow
            };

            return await _mensagemRepository.CriarAsync(mensagem);
        }
    }
}

