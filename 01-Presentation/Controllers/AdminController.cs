using _02_Application.DTOs;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
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
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AdminController(IUserService userservice,
                                        IRoleService roleService)
        {
            _userService = userservice;
            _roleService = roleService;
        }

        // Gestão de Usuários
        /// <summary>Painel de Admin - Lista todos os usuários cadastrados</summary>
        /// <param name="page">Número da página (mínimo 1, máximo 1000).</param>
        /// <param name="pageSize">Quantidade de registros por página (mínimo 1, máximo 50).</param>
        /// <returns>Retorna a lista paginada de usuários ou lista vazia.</returns>
        /// <response code="200">Ok, lista paginada de usuários.</response>
        [ProducesResponseType(200)]
        [HttpGet("listaUsuarios")]
        public async Task<IActionResult> GetAllUsersAsync(int page = 1, int pageSize = 10)
        {
            PagedResult<UserDto> result = await _userService.GetAllUsersAsync(page, pageSize);

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa um usuário pelo seu ID único</summary>
        /// <param name="id">ID único do usuário.</param>
        /// <returns>O usuário, se estiver cadastrado.</returns>
        /// <response code="200">Ok, retorna o cadastro do usuário.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaUsuarioPorId")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            UserDto? result = await _userService.GetUserByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Deleta qualquer usuário a partir do ID</summary>
        /// <param name="id">Id do Usuário.</param>
        /// <response code="204">Ok, usuário deletado.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("deletaUsuario")]
        public async Task<IActionResult> AdminDeleteUserAsync(int id)
        {
            if (!await _userService.DeleteUserByIdAsync((int)id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // Gestão de Roles/Acessos
        /// <summary>Painel de Admin - Lista todas as Roles existentes</summary>
        /// <response code="200">Retorna a lista de roles ou uma lista vazia.</response>
        [ProducesResponseType(200)]
        [HttpGet("listaRoles")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            return Ok(await _roleService.GetAllRolesAsync());
        }

        // Vínculo entre Usuário e Role
        /// <summary>Painel de Admin - Concede acesso à role definida para o usuário</summary>
        /// <param name="userId">ID único do usuário.</param>
        /// <param name="roleId">ID único da role em que será concedido acesso.</param>
        /// <response code="200">Ok, permissão concedida.</response>
        /// <response code="400">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [HttpPost("concederAcesso")]
        public async Task<IActionResult> AddRoleToUserAsync(int userId, int roleId)
        {
            if (!await _roleService.AddRoleToUserAsync(userId, roleId))
            {
                return BadRequest(new { message = "Usuário ou Role não encontrados." });
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Revoga acesso à role definida para o usuário</summary>
        /// <param name="userId">Id único do usuário.</param>
        /// <param name="roleId">ID único da role em que será removido o acesso.</param>
        /// <response code="200">Ok, acesso revogado.</response>
        /// <response code="400">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpDelete("revogarAcesso")]
        public async Task<IActionResult> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            if (!await _roleService.RemoveRoleFromUserAsync(userId, roleId))
            {
                return BadRequest(new { message = "Usuário ou Role não encontrados." });
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Revoga o acesso à todas as roles do usuário</summary>
        /// <param name="userId">Id único do usuário.</param>
        /// <response code="200">Ok, acessos removidos.</response>
        /// <response code="400">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [HttpDelete("revogarTodosAcessos")]
        public async Task<IActionResult> RemoveAllRolesFromUserAsync(int userId)
        {
            if (!await _roleService.RemoveAllRolesFromUserAsync((int)userId))
            {
                return BadRequest(new { message = "Usuário ou Role não encontrados." });
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Lista todas as roles que um usuário tem permissão de acesso</summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <returns>Lista de todas as roles do usuário.</returns>
        /// <response code="200">Ok, retorna lista com todas as roles do usuário.</response>
        /// <response code="400">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [HttpGet("exibirAcessosUsuario")]
        public async Task<IActionResult> ListUserRolesAsync(string userEmail)
        {
            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return BadRequest(new { message = "Usuário ou Role não encontrados." });
            }

            ICollection<IdLabelDto> userRoles = await _roleService.GetUserRolesAsync((int)id);
            
            if (userRoles.Count < 1)
            {
                return BadRequest(new { message = "Usuário ou Role não encontrados." });
            }
            
            return Ok(userRoles);
        }
    }
}
