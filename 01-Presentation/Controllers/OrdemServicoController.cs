using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities;
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

            return Ok(ordens);
        }

        [HttpGet("detalhe/{id}")]
        public async Task<IActionResult> Detalhe(int id)
        {
            OrdemServico? ordem =
                await _ordemServicoService.GetByIdAsync(id);

            if (ordem == null)
            {
                return NotFound("Ordem de serviço não encontrada.");
            }

            return Ok(ordem);
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar(OrdemServicoDto dto)
        {
            OrdemServico ordem =
                await _ordemServicoService.CreateAsync(dto);

            return Ok(ordem);
        }

        [HttpPut("editar")]
        public async Task<IActionResult> Editar(OrdemServicoDto dto)
        {
            bool resultado =
                await _ordemServicoService.UpdateAsync(dto);

            if (!resultado)
            {
                return NotFound("Ordem de serviço não encontrada.");
            }

            return Ok("Ordem de serviço atualizada com sucesso.");
        }
    }
}
