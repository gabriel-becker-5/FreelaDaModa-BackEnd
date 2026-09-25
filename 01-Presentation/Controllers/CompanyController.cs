using _02_Application.DTOs.Company;
using _02_Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _01_Presentation.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly IUserService _userService;

        public CompanyController(IUserService userservice)
        {
            _userService = userservice;
        }

        /// <summary>Endpoint Público para exibir o perfil público da Empresa.</summary>
        /// <param name="userId">Id único do usuário.</param>
        /// <returns>Perfil público da Empresa informada, se existir.</returns>
        /// <response code="200">Ok, retorna o perfil público da Empresa.</response>
        /// <response code="404">Empresa não encontrada.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        [HttpGet("exibirPerfilPublicoEmpresa")]
        public async Task<IActionResult> GetCompanyPublicProfileByIdAsync(int userId)
        {
            CompanyPublicProfileDto? company = await _userService.GetCompanyPublicProfileByIdAsync(userId);

            if (company != null)
            {
                return Ok(company);
            }

            return NotFound();
        }
    }
}