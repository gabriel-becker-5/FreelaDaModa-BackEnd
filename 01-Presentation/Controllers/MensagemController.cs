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
    public class MensagemController : ControllerBase
    {
        private readonly IMensagemService _mensagemService;

        public MensagemController(IMensagemService mensagemService)
        {
            _mensagemService = mensagemService;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            List<Mensagem> mensagens =
                await _mensagemService.ListarAsync();

            return Ok(mensagens);
        }

        [HttpGet("detalhe/{id}")]
        public async Task<IActionResult> Detalhe(int id)
        {
            Mensagem? mensagem =
                await _mensagemService.BuscarPorIdAsync(id);

            if (mensagem == null)
            {
                return NotFound("Mensagem não encontrada.");
            }

            return Ok(mensagem);
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> Enviar(MensagemDto dto)
        {
            Mensagem mensagem =
                await _mensagemService.CriarAsync(dto);

            return Ok(mensagem);
        }
    }
}