using _02_Application.Authorization;
using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities.UserInfo;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserController(IUserService userservice,
                                        IRoleService roleService)
        {
            _userService = userservice;
            _roleService = roleService;
        }

        private string? GetLoggedInUserEmailAddress()
        {
            return User.FindFirstValue(JwtRegisteredClaimNames.Name);
        }

        /// <summary>
        /// Cria um novo usuário na base de dados.
        /// </summary>
        /// <param name="dto">Campos: Email, Senha, Nome e Data de nascimento.</param>
        /// <returns>A conta do usuário criada.</returns>
        /// <response code="201">Conta do usuário criada com sucesso.</response>
        /// <response code="400">Informações inseridas inválidas.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("cadastrar")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Role role = await _roleService.GetRoleAsync(Roles.Default);

            if (role == null)
            {
                return BadRequest();
            }

            User newUser = await _userService.CreateUserAsync(dto);

            if (newUser == null)
            {
                return BadRequest();
            }

            await _userService.CreateUserRoleAsync(newUser, role);

            UserRegisterDto userDto = new()
            {
                Email = newUser.Email,
                Name = newUser.Name
            };

            return CreatedAtAction(nameof(GetUserByIdAsync),
                new { id = newUser.Id },
                userDto);
        }

        /// <summary>
        /// Painel de Admin - Lista todos os usuários cadastrados
        /// </summary>
        /// <returns>Retorna a lista de usuários ou lista vazia.</returns>
        /// <response code="200">Ok, lista de usuários.</response>
        /// <response code="404">Não há usuários cadastrados.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("listarTodos")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllUserAsync()
        {
            List<User>? result = await _userService.GetAllUsersAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Painel de Admin - Pesquisa um usuário pelo seu ID único.
        /// </summary>
        /// <param name="id">ID único do usuário.</param>
        /// <returns>O usuário, se estiver cadastrado.</returns>
        /// <response code="200">Ok, retorna o cadastro do usuário.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ActionName(nameof(GetUserByIdAsync))]
        [HttpGet("pesquisarPorId")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            User? result = await _userService.GetUserByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Atualiza o perfil do usuário logado.
        /// </summary>
        /// <param name="dto">Campos: Email, Nome and Data de nascimento.</param>
        /// <response code="200">Ok, perfil atualizado.</response>
        /// <response code="400">As informações inseridas são inválidas.</response>
        /// <response code="401">Acesso não autorizado, o email já está associado à outra conta.</response>
        /// <response code="404">Usuário não localizado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpPut("atualizarCadastro")]
        public async Task<IActionResult> UpdateUserAsync(UserUpdateDto dto)
        {
            string? userEmail = GetLoggedInUserEmailAddress();

            if (userEmail == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            User loggedinUser = await _userService.GetUserByEmailAsync(userEmail);

            if (loggedinUser == null)
            {
                return NotFound();
            }

            if (dto.Email != loggedinUser.Email && await _userService.IsUserEmailRegistered(dto.Email))
            {
                return Unauthorized();
            }

            await _userService.UpdateUserAsync(dto, loggedinUser);
            return Ok();
        }

        /// <summary>
        /// Deleta a conta do usuário logado.
        /// </summary>
        /// <response code="204">Ok, usuário deletado.</response>
        /// <response code="400">A informação inserida é inválida.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        [HttpDelete("deletarCadastro")]
        [Authorize(Roles = Roles.Default)]
        public async Task<IActionResult> DeleteUserAsync()
        {
            string? userEmail = GetLoggedInUserEmailAddress();

            if (userEmail == null)
            {
                return BadRequest();
            }

            User loggedinUser = await _userService.GetUserByEmailAsync(userEmail);

            if (loggedinUser == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(loggedinUser);
            return NoContent();
        }

        /// <summary>
        /// Painel de Admin - Deleta qualquer usuário a partir do e-mail.
        /// </summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <response code="204">Ok, usuário deletado.</response>
        /// <response code="400">A informação inserida é inválida.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpDelete("deletarCadastroAdmin")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AdminDeleteUserAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest();
            }

            User loggedinUser = await _userService.GetUserByEmailAsync(userEmail);

            if (loggedinUser == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(loggedinUser);
            return NoContent();
        }

        /// <summary>
        /// Painel de Admin - Lista todas as Roles existentes.
        /// </summary>
        /// <response code="200">Retorna a lista de roles ou uma lista vazia.</response>
        [ProducesResponseType(200)]
        [HttpGet("listarRoles")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            return Ok(await _roleService.GetAllRolesAsync());
        }

        /// <summary>
        /// Painel de Admin - Pesquisa a Role pelo ID único.
        /// </summary>
        /// <param name="roleId">ID única da role.</param>
        /// <returns>A role, se encontrada.</returns>
        /// <response code="200">Ok, retorna a role.</response>
        /// <response code="404">Role não encontrada.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisarRolePorId")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetRoleByIdAsync(int roleId)
        {
            Role? result = await _roleService.GetRoleIdAsync(roleId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Painel de Admin - Concede acesso à role definida para o usuário.
        /// </summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <param name="roleName">Posição em que será concedido acesso.</param>
        /// <response code="200">Ok, permissão concedida.</response>
        /// <response code="404">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [HttpPost("concederAcesso")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddRoleToUserAsync(string userEmail, string roleName)
        {
            User? user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound();
            }

            Role? role = await _roleService.GetRoleAsync(roleName);

            if (role == null)
            {
                return NotFound();
            }

            await _userService.CreateUserRoleAsync(user, role);
            return Ok();
        }

        /// <summary>
        /// Painel de Admin - Revoga acesso à role definida para o usuário.
        /// </summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <param name="roleName">Posição em que será removido o acesso.</param>
        /// <response code="200">Ok, acesso revogado.</response>
        /// <response code="404">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpDelete("revogarAcesso")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RemoveRoleFromUserAsync(string userEmail, string roleName)
        {
            User? user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound();
            }

            Role? role = await _roleService.GetRoleAsync(roleName);

            if (role == null)
            {
                return NotFound();
            }

            await _userService.RemoveRoleFromUserAsync(user, role);
            return Ok();
        }

        /// <summary>
        /// Painel de Admin - Revoga o acesso à todas as roles do usuário.
        /// </summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <response code="200">Ok, acessos removidos.</response>
        /// <response code="404">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [HttpDelete("revogarTodosAcessos")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RemoveAllRolesFromUserAsync(string userEmail)
        {
            User? user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound();
            }

            await _userService.RemoveAllRolesFromUserAsync(user);

            return Ok();
        }

        /// <summary>
        /// Painel de Admin - Lista todas as roles que um usuário tem permissão de acesso.
        /// </summary>
        /// <param name="userEmail">E-mail do usuário.</param>
        /// <returns>Lista de todas as roles do usuário.</returns>
        /// <response code="200">Ok, retorna lista com todas as roles do usuário.</response>
        /// <response code="404">Usuário ou Role não encontrados.</response>
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [HttpGet("mostrarAcessosDoUsuario")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ListUserRolesAsync(string userEmail)
        {
            User? user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound();
            }

            List<int> allUserRolesInteger = await _userService.GetUserRolesAsync(user);
            List<string?> allUserRolesString = await _roleService.GetRoleNameByIdAsync(allUserRolesInteger);
            List<UserRoleDto> allUserRoles = [];

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
    }
}