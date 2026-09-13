using _02_Application.Authorization;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
using _02_Application.Interfaces;
using _02_Application.Services;
using _04_Domain.Entities.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _01_Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = Roles.Admin)]
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
        /// <returns>Retorna a lista de usuários ou lista vazia.</returns>
        /// <response code="200">Ok, lista de usuários.</response>
        /// <response code="404">Não há usuários cadastrados.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("listaUsuarios")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            ICollection<UserDto> result = await _userService.GetAllUsersAsync();

            if (result.Count == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa um usuário pelo seu ID único</summary>
        /// <param name="id">ID único do usuário.</param>
        /// <returns>O usuário, se estiver cadastrado.</returns>
        /// <response code="200">Ok, retorna o cadastro do usuário.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ActionName(nameof(GetUserByIdAsync))]
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

        /// <summary>Painel de Admin - Deleta qualquer usuário a partir do e-mail</summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <response code="204">Ok, usuário deletado.</response>
        /// <response code="400">A informação inserida é inválida.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpDelete("deletaUsuario")]
        public async Task<IActionResult> AdminDeleteUserAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest();
            }

            int? userToDelete = await _userService.GetUserIdByEmailAsync(userEmail);

            if (userToDelete == null)
            {
                return NotFound();
            }

            if (!await _userService.DeleteUserByIdAsync((int)userToDelete))
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

        /// <summary>Painel de Admin - Pesquisa a Role pelo ID único</summary>
        /// <param name="roleId">ID única da role.</param>
        /// <returns>A role, se encontrada.</returns>
        /// <response code="200">Ok, retorna a role.</response>
        /// <response code="404">Role não encontrada.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaRolePorId")]
        public async Task<IActionResult> GetRoleByIdAsync(int roleId)
        {
            UserRoleDto? result = await _roleService.GetRoleByIdAsync(roleId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // Vínculo entre Usuário e Role
        /// <summary>Painel de Admin - Concede acesso à role definida para o usuário</summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <param name="roleName">Posição em que será concedido acesso.</param>
        /// <response code="200">Ok, permissão concedida.</response>
        /// <response code="404">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [HttpPost("concederAcesso")]
        public async Task<IActionResult> AddRoleToUserAsync(string userEmail, string roleName)
        {
            int? id = await _userService.GetUserIdByEmailAsync(userEmail);
            
            if (id == null)
            {
                return NotFound();
            }

            int? roleId = await _roleService.GetRoleIdByNameAsync(roleName);

            if (roleId == null)
            {
                return NotFound();
            }

            await _userService.CreateUserRoleAsync((int)id, (int)roleId);
            return Ok();
        }

        /// <summary>Painel de Admin - Revoga acesso à role definida para o usuário</summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <param name="roleId">ID único da role em que será removido o acesso.</param>
        /// <response code="200">Ok, acesso revogado.</response>
        /// <response code="404">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpDelete("revogarAcesso")]
        public async Task<IActionResult> RemoveRoleFromUserAsync(string userEmail, int roleId)
        {
            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return NotFound();
            }

            UserRoleDto roleDto = await _roleService.GetRoleByIdAsync(roleId);

            if (roleDto == null)
            {
                return NotFound();
            }

            await _userService.RemoveRoleFromUserAsync((int)id, roleDto.RoleId);
            return Ok();
        }

        /// <summary>Painel de Admin - Revoga o acesso à todas as roles do usuário</summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <response code="200">Ok, acessos removidos.</response>
        /// <response code="404">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [HttpDelete("revogarTodosAcessos")]
        public async Task<IActionResult> RemoveAllRolesFromUserAsync(string userEmail)
        {
            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return NotFound();
            }

            await _userService.RemoveAllRolesFromUserAsync((int)id);

            return Ok();
        }

        /// <summary>Painel de Admin - Lista todas as roles que um usuário tem permissão de acesso</summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <returns>Lista de todas as roles do usuário.</returns>
        /// <response code="200">Ok, retorna lista com todas as roles do usuário.</response>
        /// <response code="400">Não foi possível executar a solicitação.</response>
        /// <response code="404">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [HttpGet("exibirAcessosUsuario")]
        public async Task<IActionResult> ListUserRolesAsync(string userEmail)
        {
            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return NotFound();
            }

            List<int> allUserRolesInteger = (await _userService.GetUserRolesAsync((int)id)).ToList();
            List<string> allUserRolesString = (await _roleService.GetRoleNameByIdAsync(allUserRolesInteger)).ToList();
            List<UserRoleDto> allUserRoles = [];

            if (allUserRolesInteger.Count == allUserRolesString.Count)
            {
                for (int i = 0; i < allUserRolesInteger.Count; i++)
                {
                    UserRoleDto newUserRoleDto = new()
                    {
                        RoleId = allUserRolesInteger[i],
                        RoleName = allUserRolesString[i]
                    };

                    allUserRoles.Add(newUserRoleDto);
                }

                return Ok(allUserRoles);
            }

            return BadRequest(new { message = "Não foi possível executar sua solicitação." });
        }

        /// <summary>Painel de Admin - Atualiza o nome da role</summary>
        /// <param name="id">ID único da role.</param>
        /// <param name="dto">Novo nome da role.</param>
        /// <returns>A role atualizada.</returns>
        /// <response code="200">Ok, role atualizada.</response>
        /// <response code="400">Não é possível atualizar a Role informada.</response>
        /// <response code="404">Role não encontrada.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/role")]
        public async Task<IActionResult> UpdateRoleAsync(int id, UserRoleDto dto)
        {
            bool? result = await _roleService.UpdateRoleAsync(id, dto.RoleName);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return BadRequest("Não é possível atualizar a Role informada.");
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Exclui a role</summary>
        /// <param name="id">ID único da role.</param>
        /// <returns>A role excluída.</returns>
        /// <response code="204">Ok, role excluída.</response>
        /// <response code="404">A role informada não existe.</response>
        /// <response code="409">Exclusão bloqueada, a role informada está em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/role")]
        public async Task<IActionResult> DeleteRoleAsync(int id)
        {
            bool? result = await _roleService.DeleteRoleAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível excluir Role em uso.");
            }

            return NoContent();
        }
    }
}