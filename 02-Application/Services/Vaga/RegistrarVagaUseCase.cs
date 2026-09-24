<<<<<<< HEAD
﻿using _02_Application.DTOs.Vaga;
using _02_Application.Mappings;
using _03_Infrastructure.Repositories.Vaga;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using DominioVaga = _04_Domain.Entities.Vaga;
=======
﻿using _02_Application.Interfaces;
using _04_Domain.Interfaces;
>>>>>>> main

namespace _02_Application.Services.Vaga;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
}

public class RegistrarVagaUseCase
{
    private readonly IVagaRepository _vagaRepository;
    private readonly IUserRepository _userRepository;

    public RegistrarVagaUseCase(IVagaRepository vagaRepository, IUserRepository userRepository)
    {
        _vagaRepository = vagaRepository;
        _userRepository = userRepository;
    }

    public async Task<RespostavagaJson> ExecutarAsync(RequisicaoRegistrarVagaJson requisicao, string emailUsuario)
    {
        var user = await _userRepository.GetByEmailAsync(emailUsuario);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuário não encontrado.");
        }

        var entidade = new DominioVaga
        {
<<<<<<< HEAD
            Titulo = requisicao.Titulo,
            Descricao = requisicao.Descricao,
            Orcamento = Convert.ToDecimal(requisicao.Salario),
            DataPublicacao = DateTime.UtcNow,
            Ativa = true,
            UsuarioId = user.Id
        };
=======
            var usuario = await _userRepository.GetUserByEmailAsync(emailUsuario);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");
>>>>>>> main

        await _vagaRepository.AdicionarAsync(entidade);
        return entidade.ParaRespostaJson();
    }
}