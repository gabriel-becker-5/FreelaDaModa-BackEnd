using _02_Application.DTOs.User;
using _02_Application.Interfaces;
using _04_Domain.Entities.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace _01_Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AuthenticationController(ITokenService tokenservice,
                                        IUserService userservice,
                                        IRoleService roleService)
        {
            _tokenService = tokenservice;
            _userService = userservice;
            _roleService = roleService;
        }

        /// <summary>Realiza o login no sistema e gera o Token JWT</summary>
        /// <response code="200">Acesso autorizado.</response>
        /// <response code="401">Acesso não autorizado, e-mail ou senha incorretos.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            int? userId = await _userService.GetUserIdByEmailAsync(login.Email);

            if (userId == null)
            {
                return Unauthorized();
            }

            PasswordVerificationResult isPasswordCorrect = await _userService.VerifyPassword((int)userId, login.Password);

            if (isPasswordCorrect != PasswordVerificationResult.Success)
            {
                return Unauthorized();
            }

            ICollection<int> UserRoleInteger = await _userService.GetUserRolesAsync((int)userId);
            ICollection<string> UserRoleString = await _roleService.GetRoleNameByIdAsync(UserRoleInteger);

            if (!UserRoleString.Any())
            {
                return Unauthorized();
            }

            string token = _tokenService.GenerateToken(login.Email, UserRoleString);
            return Ok(new { token });
        }
    }
}