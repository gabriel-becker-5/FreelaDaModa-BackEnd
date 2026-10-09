using _02_Application.DTOs.OrdemServico;
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
    [Route("api/v{version:apiVersion}/OrdemServico")]
    [ApiController]
    [Authorize]
    public class OrdemServicoTransicaoController : ControllerBase
    {
        private readonly IOrdemServicoTransicaoService _ordemServicoTransicaoService;

        public OrdemServicoTransicaoController(IOrdemServicoTransicaoService ordemServicoTransicaoService)
        {
            _ordemServicoTransicaoService = ordemServicoTransicaoService;
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

        // POST /OrdemServico/{id}/concluir
        [HttpPost("{id:int}/concluir")]
        public async Task<IActionResult> ConcluirAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { sucesso = false, mensagem = "O id da ordem de serviço deve ser maior que zero." });
            }

            int? usuarioLogadoId = GetLoggedUserId();

            if (usuarioLogadoId == null)
            {
                return Unauthorized(new { sucesso = false, mensagem = "Não foi possível identificar o usuário logado." });
            }

            OrdemServicoTransicaoResultadoDto resultado =
                await _ordemServicoTransicaoService.ConcluirAsync(id, usuarioLogadoId.Value);

            return MapearResultado(resultado);
        }

        // POST /OrdemServico/{id}/cancelar
        [HttpPost("{id:int}/cancelar")]
        public async Task<IActionResult> CancelarAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { sucesso = false, mensagem = "O id da ordem de serviço deve ser maior que zero." });
            }

            int? usuarioLogadoId = GetLoggedUserId();

            if (usuarioLogadoId == null)
            {
                return Unauthorized(new { sucesso = false, mensagem = "Não foi possível identificar o usuário logado." });
            }

            OrdemServicoTransicaoResultadoDto resultado =
                await _ordemServicoTransicaoService.CancelarAsync(id, usuarioLogadoId.Value);

            return MapearResultado(resultado);
        }

        private IActionResult MapearResultado(OrdemServicoTransicaoResultadoDto resultado)
        {
            return resultado.Resultado switch
            {
                OrdemServicoTransicaoResultado.Sucesso =>
                    Ok(new
                    {
                        sucesso = true,
                        mensagem = resultado.Mensagem,
                        dados = new { ordemServicoId = resultado.OrdemServicoId }
                    }),

                OrdemServicoTransicaoResultado.UsuarioInativo =>
                    Unauthorized(new { sucesso = false, mensagem = resultado.Mensagem }),

                OrdemServicoTransicaoResultado.NaoEncontrada =>
                    NotFound(new { sucesso = false, mensagem = resultado.Mensagem }),

                OrdemServicoTransicaoResultado.NaoAutorizado =>
                    StatusCode(403, new { sucesso = false, mensagem = resultado.Mensagem }),

                OrdemServicoTransicaoResultado.EstadoInvalido =>
                    Conflict(new { sucesso = false, mensagem = resultado.Mensagem }),

                _ =>
                    StatusCode(500, new { sucesso = false, mensagem = "Não foi possível concluir a operação." })
            };
        }
    }
}
