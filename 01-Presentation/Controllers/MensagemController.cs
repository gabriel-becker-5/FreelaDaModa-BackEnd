using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        private int? GetLoggedUserId()
        {
            string? userId =
                User.FindFirstValue(JwtRegisteredClaimNames.NameId);

            if (int.TryParse(userId, out int id))
            {
                return id;
            }

            string? userIdAlternative =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdAlternative, out int alternativeId))
            {
                return alternativeId;
            }

            return null;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return Unauthorized("Não foi possível identificar o usuário logado.");
            }

            List<Mensagem> mensagens =
                await _mensagemService.ListarAsync(userId.Value);

            return Ok(mensagens);
        }

        [HttpGet("detalhe/{id}")]
        public async Task<IActionResult> Detalhe(int id)
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return Unauthorized("Não foi possível identificar o usuário logado.");
            }

            Mensagem? mensagem =
                await _mensagemService.BuscarPorIdAsync(id);

            if (mensagem == null)
            {
                return NotFound("Mensagem não encontrada.");
            }

            // CORREÇÃO: Validação de Propriedade (Ownership Check) - só remetente/destinatário podem ver a mensagem
            if (mensagem.RemetenteId != userId.Value && mensagem.DestinatarioId != userId.Value)
            {
                return StatusCode(403, "Você não tem permissão para acessar esta mensagem.");
            }

            return Ok(mensagem);
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> Enviar(MensagemDto dto)
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return Unauthorized("Não foi possível identificar o usuário logado.");
            }

            Mensagem mensagem =
                await _mensagemService.CriarAsync(dto, userId.Value);

            return Ok(mensagem);
        }
    }
}
