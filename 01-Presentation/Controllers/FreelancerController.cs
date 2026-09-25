using _02_Application.DTOs.Freelancer;
using _02_Application.Interfaces;
using _04_Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _01_Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = nameof(Roles.Company) + "," + nameof(Roles.Admin))]
    public class FreelancerController : ControllerBase
    {
        private readonly IUserService _userService;

        public FreelancerController(IUserService userservice)
        {
            _userService = userservice;
        }

        /// <summary>Mural de Freelancers</summary>
        /// <param name="page">Numeração da página (máximo 1000).</param>
        /// <param name="pageSize">Número de registros por página (máximo 50).</param>
        /// <returns>Lista de Freelancers cadastrados.</returns>
        /// <response code="200">Ok, retorna a lista de freelancer com cadastro ativo.</response>
        [ProducesResponseType(200)]
        [HttpGet("listarFreelancers")]
        public async Task<IActionResult> GetAllFreelancersAsync(int page = 1, int pageSize = 10)
        {
            return Ok(await _userService.GetAllFreelancersAsync(page, pageSize));
        }

        /// <summary>Endpoint Público para exibir o perfil público do Freelancer.</summary>
        /// <param name="userId">Id único do usuário.</param>
        /// <returns>Perfil público do Freelancer informado, se existir.</returns>
        /// <response code="200">Ok, retorna o perfil público do Freelancer.</response>
        /// <response code="404">Freelancer não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        [HttpGet("exibirPerfilPublicoFreelancer")]
        public async Task<IActionResult> GetFreelancerPublicProfileByIdAsync(int userId)
        {
            FreelancerPublicProfileDto? freelancer = await _userService.GetFreelancerPublicProfileByIdAsync(userId);

            if (freelancer != null)
            {
                return Ok(freelancer);
            }

            return NotFound();
        }
    }
}