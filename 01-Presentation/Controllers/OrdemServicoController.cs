using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace _01_Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class OrdemServicoController : ControllerBase
    {
        private readonly IOrdemServicoService _ordemServicoService;

        public OrdemServicoController(IOrdemServicoService ordemServicoService)
        {
            _ordemServicoService = ordemServicoService;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            List<OrdemServico> ordens =
                await _ordemServicoService.ListAllAsync();

            List<OrdemServicoResponseDto> response = ordens
                .Select(ordem => new OrdemServicoResponseDto
                {
                    Id = ordem.Id,
                    UserId = ordem.UserId,
                    Titulo = ordem.Titulo,
                    Descricao = ordem.Descricao,
                    Categoria = ordem.Categoria,
                    Modalidade = ordem.Modalidade,
                    Cidade = ordem.Cidade,
                    Valor = ordem.Valor,
                    Prazo = ordem.Prazo,
                    Status = ordem.Status,
                    Observacoes = ordem.Observacoes,
                    FreelancerId = ordem.FreelancerId
                })
                .ToList();

            return Ok(response);
        }

        [HttpGet("detalhe/{id}")]
        public async Task<IActionResult> Detalhe(int id)
        {
            try
            {
                OrdemServico? ordem =
                    await _ordemServicoService.GetByIdAsync(id);

                if (ordem == null)
                {
                    return NotFound("Ordem de serviço não encontrada.");
                }

                OrdemServicoResponseDto response = new()
                {
                    Id = ordem.Id,
                    UserId = ordem.UserId,
                    Titulo = ordem.Titulo,
                    Descricao = ordem.Descricao,
                    Categoria = ordem.Categoria,
                    Modalidade = ordem.Modalidade,
                    Cidade = ordem.Cidade,
                    Valor = ordem.Valor,
                    Prazo = ordem.Prazo,
                    Status = ordem.Status,
                    Observacoes = ordem.Observacoes,
                    FreelancerId = ordem.FreelancerId
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpPost("cadastrar")]
        [Authorize(Roles = nameof(Roles.Company))]
        public async Task<IActionResult> Cadastrar(OrdemServicoDto dto)
        {
            try
            {
                OrdemServico ordem =
                    await _ordemServicoService.CreateAsync(dto);

                OrdemServicoResponseDto response = new()
                {
                    Id = ordem.Id,
                    UserId = ordem.UserId,
                    Titulo = ordem.Titulo,
                    Descricao = ordem.Descricao,
                    Categoria = ordem.Categoria,
                    Modalidade = ordem.Modalidade,
                    Cidade = ordem.Cidade,
                    Valor = ordem.Valor,
                    Prazo = ordem.Prazo,
                    Status = ordem.Status,
                    Observacoes = ordem.Observacoes,
                    FreelancerId = ordem.FreelancerId
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpPut("editar")]
        public async Task<IActionResult> Editar(OrdemServicoDto dto)
        {
            try
            {
                bool resultado =
                    await _ordemServicoService.UpdateAsync(dto);

                if (!resultado)
                {
                    return NotFound("Ordem de serviço não encontrada.");
                }

                return Ok("Ordem de serviço atualizada com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            bool excluido =
                await _ordemServicoService.DeleteAsync(id);

            if (!excluido)
            {
                return NotFound("Ordem de serviço não encontrada ou não pertence ao usuário.");
            }

            return Ok("Ordem de serviço excluída com sucesso.");
        }
    }
}
