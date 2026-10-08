using _02_Application.Services.Dashboard;
using _04_Domain.Enums;
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
    public class DashboardController : ControllerBase
    {
        private readonly ObterDashboardFreelancerUseCase _obterDashboardFreelancerUseCase;
        private readonly ObterDashboardEmpresaUseCase _obterDashboardEmpresaUseCase;

        public DashboardController(
            ObterDashboardFreelancerUseCase obterDashboardFreelancerUseCase,
            ObterDashboardEmpresaUseCase obterDashboardEmpresaUseCase)
        {
            _obterDashboardFreelancerUseCase = obterDashboardFreelancerUseCase;
            _obterDashboardEmpresaUseCase = obterDashboardEmpresaUseCase;
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

        /// <summary>Painel do Freelancer: perfil, avaliações, vagas recomendadas, produções ativas e conversas recentes.</summary>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("freelancer")]
        [Authorize(Roles = nameof(Roles.Freelancer))]
        public async Task<IActionResult> GetFreelancerDashboardAsync()
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Não foi possível identificar o usuário logado." });
            }

            var resultado = await _obterDashboardFreelancerUseCase.ExecutarAsync(userId.Value);
            return Ok(resultado);
        }

        /// <summary>Painel da Empresa: OS ativas, vagas publicadas, candidaturas recentes e conversas recentes.</summary>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("empresa")]
        [Authorize(Roles = nameof(Roles.Company))]
        public async Task<IActionResult> GetCompanyDashboardAsync()
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Não foi possível identificar o usuário logado." });
            }

            var resultado = await _obterDashboardEmpresaUseCase.ExecutarAsync(userId.Value);
            return Ok(resultado);
        }
    }
}
