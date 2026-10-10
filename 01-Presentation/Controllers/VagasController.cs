using _02_Application.DTOs.Vaga;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _04_Domain.Enums;
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
    public class VagasController : ControllerBase
    {
        private readonly IVagaService _vagaService;

        public VagasController(IVagaService vagaService)
        {
            _vagaService = vagaService;
        }

        private string? GetLoggedUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
        }

        private int? GetLoggedUserId()
        {
            string? claim = User.FindFirstValue(JwtRegisteredClaimNames.NameId);
            return int.TryParse(claim, out int id) && id > 0 ? id : (int?)null;
        }

        // POST /vagas
        [HttpPost]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> CreateVagaAsync([FromBody] RequisicaoRegistrarVagaJson dto)
        {
            var usuarioId = GetLoggedUserId();
            if (usuarioId is null)
            {
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." });
            }

            if (dto.Orcamento < 0.01m || dto.Orcamento > 9999999999999999.99m)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O orçamento deve ser maior que zero e respeitar o limite permitido."
                });
            }

            if (decimal.Round(dto.Orcamento, 2) != dto.Orcamento)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O orçamento deve ter no máximo duas casas decimais."
                });
            }

            DateTime prazoMinimo = new DateTime(1000, 1, 1);
            DateTime prazoMaximo = new DateTime(9999, 12, 31, 23, 59, 59)
                .AddTicks(4999990);

            if (!dto.PrazoConclusao.HasValue ||
                dto.PrazoConclusao.Value < prazoMinimo ||
                dto.PrazoConclusao.Value > prazoMaximo)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "Informe um prazo de conclusão válido."
                });
            }

            try
            {
                var resposta = await _vagaService.RegistrarAsync(dto, usuarioId.Value);
                return Ok(new { sucesso = true, dados = resposta });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { sucesso = false, mensagem = ex.Message });
            }
        }

        // GET /vagas?empresaId=&usuarioId=&status=
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetVagasAsync(
            [FromQuery] int? empresaId,
            [FromQuery] int? usuarioId,
            [FromQuery] StatusVaga? status)
        {
            var idFiltro = empresaId ?? usuarioId;

            var vagas = await _vagaService.ListarAsync(idFiltro, status);

            return Ok(new { sucesso = true, dados = vagas });
        }

        // GET /vagas/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVagaByIdAsync(int id)
        {
            var vaga = await _vagaService.BuscarPorIdAsync(id);

            if (vaga == null)
            {
                return NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." });
            }

            return Ok(new { sucesso = true, dados = vaga });
        }

        // PATCH /vagas/{id} (editar)
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateVagaAsync(int id, [FromBody] UpdateVagaDto dto)
        {
            var userEmail = GetLoggedUserEmail();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." });
            }

            if (dto.Titulo != null && string.IsNullOrWhiteSpace(dto.Titulo))
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O título não pode estar vazio."
                });
            }

            if (dto.Descricao != null && string.IsNullOrWhiteSpace(dto.Descricao))
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "A descrição não pode estar vazia."
                });
            }

            if (dto.Cidade != null && string.IsNullOrWhiteSpace(dto.Cidade))
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "A cidade não pode estar vazia."
                });
            }

            if (dto.Estado != null && string.IsNullOrWhiteSpace(dto.Estado))
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O estado não pode estar vazio."
                });
            }

            if (dto.Orcamento.HasValue)
            {
                decimal orcamento = dto.Orcamento.Value;

                if (orcamento < 0.01m || orcamento > 9999999999999999.99m)
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        mensagem = "O orçamento deve ser maior que zero e respeitar o limite permitido."
                    });
                }

                if (decimal.Round(orcamento, 2) != orcamento)
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        mensagem = "O orçamento deve ter no máximo duas casas decimais."
                    });
                }
            }

            if (dto.PrazoConclusao.HasValue)
            {
                DateTime prazoMinimo = new DateTime(1000, 1, 1);
                DateTime prazoMaximo = new DateTime(9999, 12, 31, 23, 59, 59)
                    .AddTicks(4999990);

                if (dto.PrazoConclusao.Value < prazoMinimo ||
                    dto.PrazoConclusao.Value > prazoMaximo)
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        mensagem = "Informe um prazo de conclusão válido."
                    });
                }
            }

            VagaOperacaoResult result =
                await _vagaService.EditarAsync(userEmail, id, dto);

            return result.Status switch
            {
                VagaOperacaoStatus.Success =>
                    Ok(new { sucesso = true, mensagem = "Vaga atualizada com sucesso.", dados = result.Vaga }),

                VagaOperacaoStatus.UsuarioNaoEncontrado =>
                    Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." }),

                VagaOperacaoStatus.NaoEncontrada =>
                    NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." }),

                VagaOperacaoStatus.SemPermissao =>
                    StatusCode(403, new { sucesso = false, mensagem = "Você não tem permissão para alterar esta vaga pois ela pertence a outra empresa." }),

                _ =>
                    StatusCode(500, new { sucesso = false, mensagem = "Não foi possível atualizar a vaga. Tente novamente." })
            };
        }

        // PATCH /vagas/{id}/status (encerrar/pausar)
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateVagaStatusAsync(int id, [FromBody] UpdateVagaStatusDto dto)
        {
            var userEmail = GetLoggedUserEmail();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." });
            }

            if (!dto.Status.HasValue)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O status da vaga é obrigatório."
                });
            }

            VagaOperacaoResult result =
                await _vagaService.AlterarStatusAsync(userEmail, id, dto.Status.Value);

            return result.Status switch
            {
                VagaOperacaoStatus.Success =>
                    Ok(new { sucesso = true, mensagem = "Status da vaga atualizado com sucesso.", dados = result.Vaga }),

                VagaOperacaoStatus.UsuarioNaoEncontrado =>
                    Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." }),

                VagaOperacaoStatus.NaoEncontrada =>
                    NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." }),

                VagaOperacaoStatus.SemPermissao =>
                    StatusCode(403, new { sucesso = false, mensagem = "Você não tem permissão para alterar o status desta vaga." }),

                _ =>
                    StatusCode(500, new { sucesso = false, mensagem = "Não foi possível atualizar o status. Tente novamente." })
            };
        }

        // DELETE /vagas/{id} (soft delete, só empresa)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> DeleteVagaAsync(int id)
        {
            var userEmail = GetLoggedUserEmail();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." });
            }

            VagaOperacaoResult result =
                await _vagaService.ExcluirAsync(userEmail, id);

            return result.Status switch
            {
                VagaOperacaoStatus.Success =>
                    NoContent(),

                VagaOperacaoStatus.UsuarioNaoEncontrado =>
                    Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." }),

                VagaOperacaoStatus.NaoEncontrada =>
                    NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." }),

                VagaOperacaoStatus.SemPermissao =>
                    StatusCode(403, new { sucesso = false, mensagem = "Você não tem permissão para excluir esta vaga." }),

                _ =>
                    StatusCode(500, new { sucesso = false, mensagem = "Não foi possível excluir a vaga. Tente novamente." })
            };
        }
    }
}