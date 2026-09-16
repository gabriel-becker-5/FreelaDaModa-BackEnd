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
    public class AvaliacaoController : ControllerBase
    {
        private readonly IAvaliacaoService _avaliacaoService;

        public AvaliacaoController(IAvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> Enviar(AvaliacaoDto dto)
        {
            Avaliacao avaliacao =
                await _avaliacaoService.CreateAsync(dto);

            return Ok(avaliacao);
        }

        [HttpGet("listar/{ordemServicoId}")]
        public async Task<IActionResult> Listar(int ordemServicoId)
        {
            List<Avaliacao> avaliacoes =
                await _avaliacaoService
                    .ListByOrdemServicoAsync(ordemServicoId);

            return Ok(avaliacoes);
        }
    }
}
