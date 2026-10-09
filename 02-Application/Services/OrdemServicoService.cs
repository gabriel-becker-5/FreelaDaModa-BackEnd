using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using _04_Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_Application.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public OrdemServicoService(
        IOrdemServicoRepository ordemServicoRepository,
        IUserService userService,
        IUserRepository userRepository)
        {
            _ordemServicoRepository = ordemServicoRepository;
            _userService = userService;
            _userRepository = userRepository;
        }

        public async Task<List<OrdemServico>> ListAllAsync()
        {
            string email = _userService.GetLoggedUserEmailAddress();

            User? usuario = await _userService.GetUserByEmailAsync(email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            return await _ordemServicoRepository.ListByUserIdAsync(usuario.Id);
        }

        public async Task<OrdemServico?> GetByIdAsync(int id)
        {
            OrdemServico? ordemServico =
                await _ordemServicoRepository.GetByIdAsync(id);

            if (ordemServico == null)
            {
                return null;
            }

            string email = _userService.GetLoggedUserEmailAddress();

            User? usuario = await _userService.GetUserByEmailAsync(email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            if (ordemServico.UserId != usuario.Id && ordemServico.FreelancerId != usuario.Id)
            {
                throw new UnauthorizedAccessException(
                    "Você não tem permissão para acessar esta ordem de serviço.");
            }

            return ordemServico;
        }

        public async Task<OrdemServico> CreateAsync(OrdemServicoDto dto)
        {
            if (dto.Valor < 0)
            {
                throw new ArgumentException("O valor da ordem de serviço não pode ser negativo.");
            }

            if (dto.Prazo < DateTime.Now)
            {
                throw new ArgumentException("O prazo da ordem de serviço não pode estar no passado.");
            }

            if (dto.FreelancerId.HasValue)
            {
                await ValidarFreelancerAsync(dto.FreelancerId.Value);
            }

            string email = _userService.GetLoggedUserEmailAddress();

            User? usuario = await _userService.GetUserByEmailAsync(email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            OrdemServico ordemServico = new()
            {
                UserId = usuario.Id,
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Categoria = dto.Categoria,
                Modalidade = dto.Modalidade,
                Cidade = dto.Cidade,
                Valor = dto.Valor,
                Prazo = dto.Prazo,
                Status = StatusOrdemServico.EmAndamento.ParaTexto(),
                Observacoes = dto.Observacoes,
                FreelancerId = dto.FreelancerId
            };

            return await _ordemServicoRepository.CreateAsync(ordemServico);
        }

        public async Task<bool> UpdateAsync(OrdemServicoDto dto)
        {
            if (dto.Valor < 0)
            {
                throw new ArgumentException("O valor da ordem de serviço não pode ser negativo.");
            }

            if (dto.Prazo < DateTime.Now)
            {
                throw new ArgumentException("O prazo da ordem de serviço não pode estar no passado.");
            }

            OrdemServico? ordemServico =
                await _ordemServicoRepository.GetByIdAsync(dto.Id);

            if (ordemServico == null)
            {
                return false;
            }

            string email = _userService.GetLoggedUserEmailAddress();

            User? usuario = await _userService.GetUserByEmailAsync(email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            if (ordemServico.UserId != usuario.Id)
            {
                return false;
            }

            if (dto.FreelancerId.HasValue && dto.FreelancerId != ordemServico.FreelancerId)
            {
                throw new ArgumentException(
                    "O freelancer da ordem de serviço não pode ser alterado por esta operação.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (!StatusOrdemServicoExtensions.TryParse(dto.Status, out StatusOrdemServico status))
                {
                    throw new ArgumentException(
                        "O status da ordem de serviço deve ser um dos seguintes valores: " +
                        $"\"{StatusOrdemServicoExtensions.TextoEmAndamento}\", " +
                        $"\"{StatusOrdemServicoExtensions.TextoConcluida}\" ou " +
                        $"\"{StatusOrdemServicoExtensions.TextoCancelada}\".");
                }

                ordemServico.Status = status.ParaTexto();
            }

            ordemServico.Titulo = dto.Titulo;
            ordemServico.Descricao = dto.Descricao;
            ordemServico.Categoria = dto.Categoria;
            ordemServico.Modalidade = dto.Modalidade;
            ordemServico.Cidade = dto.Cidade;
            ordemServico.Valor = dto.Valor;
            ordemServico.Prazo = dto.Prazo;
            ordemServico.Observacoes = dto.Observacoes;
            // FreelancerId e Status não são sobrescritos diretamente pelo dto (ver validações acima).

            await _ordemServicoRepository.UpdateAsync(ordemServico);

            return true;
        }

        private async Task ValidarFreelancerAsync(int freelancerId)
        {
            User? freelancer = await _userRepository.GetUserByIdAsync(freelancerId);

            if (freelancer == null)
            {
                throw new ArgumentException("O freelancer informado não foi encontrado.");
            }

            if (freelancer.IsDeleted)
            {
                throw new ArgumentException("O freelancer informado não está mais disponível.");
            }

            if (freelancer.Roles == null || !freelancer.Roles.Contains(Roles.Freelancer))
            {
                throw new ArgumentException("O usuário informado não possui o perfil de freelancer.");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            OrdemServico? ordemServico =
                await _ordemServicoRepository.GetByIdAsync(id);

            if (ordemServico == null)
            {
                return false;
            }

            string email = _userService.GetLoggedUserEmailAddress();

            User? usuario = await _userService.GetUserByEmailAsync(email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            if (ordemServico.UserId != usuario.Id)
            {
                return false;
            }

            await _ordemServicoRepository.DeleteAsync(ordemServico);

            return true;
        }
    }
}
