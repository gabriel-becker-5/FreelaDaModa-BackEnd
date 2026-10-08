using _02_Application.DTOs.Vaga;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _04_Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // POST /vagas
        [HttpPost]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> CreateVagaAsync([FromBody] RequisicaoRegistrarVagaJson dto)
        {
            var userEmail = GetLoggedUserEmail();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { sucesso = false, mensagem = "E-mail não encontrado no token JWT." });
            }

            try
            {
                var resposta = await _vagaService.RegistrarAsync(dto, userEmail);
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

            VagaOperacaoResult result =
                await _vagaService.EditarAsync(userEmail, id, dto.Titulo, dto.Descricao);

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

            VagaOperacaoResult result =
                await _vagaService.AlterarStatusAsync(userEmail, id, dto.Status);

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

    public class UpdateVagaDto
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
    }

    public class UpdateVagaStatusDto
    {
        public StatusVaga Status { get; set; }
    }
}