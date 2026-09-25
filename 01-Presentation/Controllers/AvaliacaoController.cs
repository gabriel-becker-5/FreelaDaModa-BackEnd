using _02_Application.DTOs;
using _02_Application.Interfaces;
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
            var avaliacao = await _avaliacaoService.CreateAsync(dto);

            var response = new AvaliacaoResponseDto
            {
                Id = avaliacao.Id,
                OrdemServicoId = avaliacao.OrdemServicoId,
                UserId = avaliacao.UserId,
                Nota = avaliacao.Nota,
                Comentario = avaliacao.Comentario
            };

            return Ok(response);
        }

        [HttpGet("listar/{ordemServicoId}")]
        public async Task<IActionResult> Listar(int ordemServicoId)
        {
            var avaliacoes = await _avaliacaoService
                .ListByOrdemServicoAsync(ordemServicoId);

            var response = avaliacoes.Select(avaliacao => new AvaliacaoResponseDto
            {
                Id = avaliacao.Id,
                OrdemServicoId = avaliacao.OrdemServicoId,
                UserId = avaliacao.UserId,
                Nota = avaliacao.Nota,
                Comentario = avaliacao.Comentario
            }).ToList();

            return Ok(response);
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Editar(int id, AvaliacaoDto dto)
        {
            bool atualizado = await _avaliacaoService.UpdateAsync(id, dto);

            if (!atualizado)
            {
                return NotFound("Avaliação não encontrada ou não pertence ao usuário.");
            }

            return Ok("Avaliação atualizada com sucesso.");
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            bool excluido = await _avaliacaoService.DeleteAsync(id);

            if (!excluido)
            {
                return NotFound("Avaliação não encontrada ou não pertence ao usuário.");
            }

            return Ok("Avaliação excluída com sucesso.");
        }
    }
}