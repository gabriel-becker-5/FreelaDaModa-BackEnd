using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using _02_Application.Services.Vaga;
using _02_Application.DTOs.Vaga;
using _02_Application.Interfaces;

namespace _01_Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VagasController : ControllerBase
    {
        private readonly IVagaService _vagaService;

        public VagasController(IVagaService vagaService)
        {
            _vagaService = vagaService;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RequisicaoRegistrarVagaJson requisicao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailUsuario = GetLoggedUserEmailAddress();
            if (string.IsNullOrEmpty(emailUsuario))
                return Unauthorized();

            var resposta = await _vagaService.RegistrarAsync(requisicao, emailUsuario);

            return Created(string.Empty, resposta);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObterComFiltros([FromQuery] int? usuarioId, [FromQuery] string? status)
        {
            var vagas = await _vagaService.ObterComFiltrosAsync(usuarioId, status);
            return Ok(vagas);
        }

        [HttpGet("mural")]
        [AllowAnonymous]
        public async Task<IActionResult> ListarMuralDeVagas()
        {
            var vagas = await _vagaService.ObterTodasAsync();
            return Ok(new
            {
                sucesso = true,
                mensagem = "Mural de vagas obtido com sucesso.",
                total = vagas?.Count() ?? 0,
                dados = vagas
            });
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var vaga = await _vagaService.ObterPorIdAsync(id);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });
            return Ok(vaga);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] RequisicaoAtualizarVagaJson requisicao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var atualizada = await _vagaService.AtualizarAsync(id, requisicao);
            if (atualizada == null)
                return NotFound(new { mensagem = "Vaga não encontrada para atualização." });
            return Ok(atualizada);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] RequisicaoAtualizarStatusVagaJson requisicao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sucesso = await _vagaService.AtualizarStatusAsync(id, requisicao.Status);
            if (!sucesso)
                return NotFound(new { mensagem = "Vaga não encontrada para alteração de status." });
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var sucesso = await _vagaService.DeletarAsync(id);
            if (!sucesso)
                return NotFound(new { mensagem = "Vaga não encontrada para remoção." });
            return NoContent();
        }

        private string? GetLoggedUserEmailAddress()
        {
            return User.FindFirstValue(JwtRegisteredClaimNames.Name) ?? User.FindFirstValue(ClaimTypes.Name);
        }
    }
}