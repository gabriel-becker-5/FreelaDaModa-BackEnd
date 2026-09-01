using _02_Application.Interfaces;
using _04_Domain.Entities.UserInfo;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace _01_Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenService _tokenservice;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AuthenticationController(ITokenService tokenservice,
                                        IUserService userservice,
                                        IRoleService roleService)
        {
            _tokenservice = tokenservice;
            _userService = userservice;
            _roleService = roleService;
        }

        /// <summary>
        /// Realiza o login no sistema e gera o Token JWT.
        /// </summary>
        /// <response code="200">Acesso autorizado.</response>
        /// <response code="401">Acesso não autorizado, e-mail ou senha incorretos.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            User? user = await _userService.GetUserByEmailAsync(login.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            PasswordVerificationResult isPasswordCorrect = _userService.VerifyPassword(user, login.Password);

            if (isPasswordCorrect != PasswordVerificationResult.Success)
            {
                return Unauthorized();
            }

            List<int> UserRolesInteger = await _userService.GetUserRolesAsync(user);
            List<string> UserRolesString = await _roleService.GetRoleNameByIdAsync(UserRolesInteger);
            string token = _tokenservice.GenerateToken(login.Email, UserRolesString);

            return Ok(new { token });
        }
    }
}