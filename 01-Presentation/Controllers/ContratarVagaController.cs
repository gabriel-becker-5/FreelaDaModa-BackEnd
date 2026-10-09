using _02_Application.DTOs.Vaga;
using _02_Application.Enums;
using _02_Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace _01_Presentation.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/vagas")]
    [ApiController]
    [Authorize]
    public class ContratarVagaController : ControllerBase
    {
        private readonly IContratarVagaService _contratarVagaService;

        public ContratarVagaController(IContratarVagaService contratarVagaService)
        {
            _contratarVagaService = contratarVagaService;
        }

        private int? GetLoggedUserId()
        {
            string? userId = User.FindFirstValue(JwtRegisteredClaimNames.NameId);

            if (int.TryParse(userId, out int id))
            {
                return id;
            }

            string? userIdAlternative = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdAlternative, out int alternativeId))
            {
                return alternativeId;
            }

            return null;
        }

        // POST /vagas/{vagaId}/contratar
        [HttpPost("{vagaId:int}/contratar")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> ContratarAsync(int vagaId, [FromBody] ContratarVagaDto? dto)
        {
            if (dto == null || dto.FreelancerId <= 0)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O freelancerId é obrigatório e deve ser maior que zero."
                });
            }

            int? usuarioLogadoId = GetLoggedUserId();

            if (usuarioLogadoId == null)
            {
                return Unauthorized(new
                {
                    sucesso = false,
                    mensagem = "Não foi possível identificar o usuário logado."
                });
            }

            ContratarVagaResultadoDto resultado = await _contratarVagaService.ContratarAsync(
                vagaId, usuarioLogadoId.Value, dto.FreelancerId);

            return resultado.Resultado switch
            {
                ContratarVagaResultado.Sucesso =>
                    Ok(new
                    {
                        sucesso = true,
                        mensagem = resultado.Mensagem,
                        dados = new { ordemServicoId = resultado.OrdemServicoId }
                    }),

                ContratarVagaResultado.VagaNaoEncontrada =>
                    NotFound(new { sucesso = false, mensagem = resultado.Mensagem }),

                ContratarVagaResultado.NaoAutorizado =>
                    StatusCode(403, new { sucesso = false, mensagem = resultado.Mensagem }),

                ContratarVagaResultado.VagaEncerrada =>
                    Conflict(new { sucesso = false, mensagem = resultado.Mensagem }),

                ContratarVagaResultado.FreelancerInvalido =>
                    BadRequest(new { sucesso = false, mensagem = resultado.Mensagem }),

                ContratarVagaResultado.CandidaturaNaoEncontrada =>
                    NotFound(new { sucesso = false, mensagem = resultado.Mensagem }),

                ContratarVagaResultado.CandidaturaRejeitada =>
                    Conflict(new { sucesso = false, mensagem = resultado.Mensagem }),

                ContratarVagaResultado.PrazoNoPassado =>
                    BadRequest(new { sucesso = false, mensagem = resultado.Mensagem }),

                _ =>
                    StatusCode(500, new { sucesso = false, mensagem = "Não foi possível concluir a contratação." })
            };
        }
    }
}
