using _02_Application.DTOs.User;
using _02_Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
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

        public AuthenticationController(ITokenService tokenservice,
                                        IUserService userservice)
        {
            _tokenService = tokenservice;
            _userService = userservice;
        }

        /// <summary>Realiza o login no sistema e gera o Token JWT</summary>
        /// <param name="login">Credenciais de acesso: E-mail e senha.</param>
        /// <response code="200">Acesso autorizado, retorna o token JWT.</response>
        /// <response code="400">As informações inseridas são inválidas.</response>
        /// <response code="401">Acesso não autorizado, e-mail ou senha incorretos.</response>
        /// <response code="429">Limite de tentativas de login excedido.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(429)]
        public async Task<IActionResult> Login(LoginDto login)
        {
            AuthenticatedUserDto? authenticatedUser = await _userService.AuthenticateAsync(login.Email, login.Password);

            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            string token = _tokenService.GenerateToken(authenticatedUser.UserId, authenticatedUser.Email, authenticatedUser.Roles);

            return Ok(new { token });
        }
    }
}