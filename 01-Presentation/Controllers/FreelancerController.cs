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

        /// <summary>Mural de Vagas</summary>
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
    }
}