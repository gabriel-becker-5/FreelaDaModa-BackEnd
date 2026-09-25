using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Interfaces;

namespace _02_Application.Services
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;
        private readonly IUserService _userService;
        private readonly IOrdemServicoRepository _ordemServicoRepository;

        public AvaliacaoService(
    IAvaliacaoRepository avaliacaoRepository,
    IUserService userService,
    IOrdemServicoRepository ordemServicoRepository)
        {
            _avaliacaoRepository = avaliacaoRepository;
            _userService = userService;
            _ordemServicoRepository = ordemServicoRepository;
        }

        public async Task<Avaliacao> CreateAsync(AvaliacaoDto dto)
        {
            if (dto.Nota < 1 || dto.Nota > 5)
            {
                throw new ArgumentException("A nota deve estar entre 1 e 5.");
            }

            OrdemServico? ordemServico =
                await _ordemServicoRepository.GetByIdAsync(dto.OrdemServicoId);

            if (ordemServico == null)
            {
                throw new ArgumentException("Ordem de serviço não encontrada.");
            }

            string email = _userService.GetLoggedUserEmailAddress();

            User? usuario = await _userService.GetUserByEmailAsync(email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            Avaliacao avaliacao = new()
            {
                OrdemServicoId = dto.OrdemServicoId,
                UserId = usuario.Id,
                Nota = dto.Nota,
                Comentario = dto.Comentario
            };

            return await _avaliacaoRepository.CreateAsync(avaliacao);
        }

        public async Task<List<Avaliacao>> ListByOrdemServicoAsync(int ordemServicoId)
        {
            return await _avaliacaoRepository
                .ListByOrdemServicoAsync(ordemServicoId);
        }
        public async Task<bool> UpdateAsync(int id, AvaliacaoDto dto)
        {
            if (dto.Nota < 1 || dto.Nota > 5)
            {
                throw new ArgumentException("A nota deve estar entre 1 e 5.");
            }

            Avaliacao? avaliacao =
                await _avaliacaoRepository.GetByIdAsync(id);

            if (avaliacao == null)
            {
                return false;
            }

            string email = _userService.GetLoggedUserEmailAddress();

            User? usuario = await _userService.GetUserByEmailAsync(email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            if (avaliacao.UserId != usuario.Id)
            {
                return false;
            }

            OrdemServico? ordemServico =
                await _ordemServicoRepository.GetByIdAsync(dto.OrdemServicoId);

            if (ordemServico == null)
            {
                throw new ArgumentException("Ordem de serviço não encontrada.");
            }

            avaliacao.OrdemServicoId = dto.OrdemServicoId;
            avaliacao.Nota = dto.Nota;
            avaliacao.Comentario = dto.Comentario;

            await _avaliacaoRepository.UpdateAsync(avaliacao);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Avaliacao? avaliacao = await _avaliacaoRepository.GetByIdAsync(id);

            if (avaliacao == null)
            {
                return false;
            }

            string email = _userService.GetLoggedUserEmailAddress();

            User? usuario = await _userService.GetUserByEmailAsync(email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            if (avaliacao.UserId != usuario.Id)
            {
                return false;
            }

            await _avaliacaoRepository.DeleteAsync(avaliacao);

            return true;
        }
    }
}