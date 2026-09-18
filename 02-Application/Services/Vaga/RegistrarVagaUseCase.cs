using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _02_Application.DTOs.Vaga;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Interfaces;

namespace _02_Application.Services.Vaga
{
    public class RegistrarVagaUseCase : IVagaService
    {
        private readonly IVagaRepository _vagaRepository;
        private readonly IUserRepository _userRepository;

        public RegistrarVagaUseCase(IVagaRepository vagaRepository, IUserRepository userRepository)
        {
            _vagaRepository = vagaRepository;
            _userRepository = userRepository;
        }

        public async Task<object> RegistrarAsync(RequisicaoRegistrarVagaJson requisicao, string emailUsuario)
        {
            var usuario = await _userRepository.GetByEmailAsync(emailUsuario);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            var vaga = new _04_Domain.Entities.Vaga
            {
                Titulo = requisicao.Titulo,
                Descricao = requisicao.Descricao,
                Orcamento = requisicao.Salario, // Ajustado para o campo correto da entidade
                UsuarioId = usuario.Id          // Ajustado para o campo correto da entidade
            };

            await _vagaRepository.AdicionarAsync(vaga);

            return new { Mensagem = "Vaga cadastrada com sucesso!" };
        }

        public async Task<IEnumerable<object>> ObterTodasAsync()
        {
            var vagas = await _vagaRepository.ObterTodasAsync();
            return vagas;
        }
    }
}