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
        public async Task<IActionResult> ObterTodas()
        {
            var vagas = await _vagaService.ObterTodasAsync();
            return Ok(vagas);
        }

        [HttpGet("mural")]
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

        private string? GetLoggedUserEmailAddress()
        {
            return User.FindFirstValue(JwtRegisteredClaimNames.Name) ?? User.FindFirstValue(ClaimTypes.Name);
        }
    }
}