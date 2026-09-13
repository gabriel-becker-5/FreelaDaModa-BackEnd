using _02_Application.Authorization;
using _02_Application.DTOs;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.Interfaces;
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

        public UserController(IUserService userservice)
        {
            _userService = userservice;
        }

        private string? GetLoggedUserEmailAddress()
        {
            return User.FindFirstValue(JwtRegisteredClaimNames.Name);
        }

        // Criação
        /// <summary>Cria um novo usuário do tipo Freelancer</summary>
        /// <param name="dto">Campos: Nome completo do responsável legal, CPF do responsável legal, E-mail, Senha, Telefone, CEP, Endereço, Número, Bairro, Cidade, Estado, Complemento, Descrição Pública do Perfil, Data de nascimento, Tipo de negócio, Tempo de experiência, Tamanho da oficina, Especialidades, Máquinas que possui, Como costuma fecha serviços, Disponibilidade de tempo, Preferências do Freelancer, Faturamento médio, Já tem produtor fixo?, Possui veículo para buscar/entregar as peças?</param>
        /// <returns>Conta/perfil do usuário criada.</returns>
        /// <response code="201">Conta/perfil criada com sucesso.</response>
        /// <response code="400">Informações inseridas inválidas.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("cadastrar/freelancer")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateFreelancerAsync(CreateFreelancerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int? newUser = await _userService.RegisterFreelancerAsync(dto);

            if (newUser == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Cria um novo usuário do tipo Empresa/Confecção</summary>
        /// <param name="dto">Campos: Nome completo do responsável legal, CPF do responsável legal, E-mail, Senha, Telefone, CEP, Endereço, Número, Bairro, Cidade, Estado, Complemento, Descrição Pública do Perfil, Razão Social, Nome Fantasia, CNPJ, Ramo de atuação.</param>
        /// <returns>Conta/perfil do usuário criada.</returns>
        /// <response code="201">Conta/perfil criada com sucesso.</response>
        /// <response code="400">Informações inseridas inválidas.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("cadastrar/empresa")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCompanyAsync(CreateCompanyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int? newUser = await _userService.RegisterCompanyAsync(dto);

            if (newUser == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }


        // Leitura
        /// <summary>Obtém o perfil completo do usuário logado - Freelancer </summary>
        /// <response code="200">Ok, retorna o perfil do usuário logado.</response>
        /// <response code="400">Usuário inválido.</response>
        /// <response code="404">Usuário não localizado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("perfil/freelancer")]
        [Authorize(Roles = Roles.Freelancer)]
        public async Task<IActionResult> GetUserFreelancerAsync()
        {
            string? userEmail = GetLoggedUserEmailAddress();

            if (userEmail == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            GetFreelancerDto? dtoLoggedUser = await _userService.GetFreelancerProfileByEmailAsync(userEmail);

            if (dtoLoggedUser == null)
            {
                return NotFound();
            }

            return Ok(dtoLoggedUser);
        }

        /// <summary>Obtém o perfil completo do usuário logado - Empresa/Confecção </summary>
        /// <response code="200">Ok, retorna o perfil do usuário logado.</response>
        /// <response code="400">Usuário inválido.</response>
        /// <response code="404">Usuário não localizado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("perfil/empresa")]
        [Authorize(Roles = Roles.Company)]
        public async Task<IActionResult> GetUserCompanyAsync()
        {
            string? userEmail = GetLoggedUserEmailAddress();

            if (userEmail == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            GetCompanyDto? dtoLoggedUser = await _userService.GetCompanyProfileByEmailAsync(userEmail);

            if (dtoLoggedUser == null)
            {
                return NotFound();
            }

            return Ok(dtoLoggedUser);
        }


        // Atualização
        /// <summary>Atualiza o perfil do usuário logado - Freelancer </summary>
        /// <param name="dto">Campos: Nome completo do responsável legal, CPF do responsável legal, E-mail, Telefone, CEP, Endereço, Número, Bairro, Cidade, Estado, Complemento, Descrição Pública do Perfil, Data de nascimento, Tipo de negócio, Tempo de experiência, Tamanho da oficina, Especialidades, Máquinas que possui, Como costuma fecha serviços, Disponibilidade de tempo, Preferências do Freelancer, Faturamento médio, Já tem produtor fixo?, Possui veículo para buscar/entregar as peças?</param>
        /// <response code="200">Ok, perfil atualizado.</response>
        /// <response code="400">As informações inseridas são inválidas.</response>
        /// <response code="401">Acesso não autorizado, o email já está associado à outra conta.</response>
        /// <response code="404">Usuário não localizado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpPut("atualizarcadastro/freelancer")]
        [Authorize(Roles = Roles.Freelancer)]
        public async Task<IActionResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto)
        {
            string? userEmail = GetLoggedUserEmailAddress();

            if (userEmail == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Os dados informados são inválidos." });
            }

            ProfileUpdateResult result = await _userService.UpdateUserFreelancerAsync(dto, userEmail);

            return result switch
            {
                ProfileUpdateResult.Success => Ok(),
                ProfileUpdateResult.NotFound => NotFound(),
                ProfileUpdateResult.EmailInUse => Unauthorized(),
                _ => BadRequest(new { message = "Os dados informados são inválidos." })
            };
        }

        /// <summary>Atualiza o perfil do usuário logado - Empresa/Confecção</summary>
        /// <param name="dto">Campos: Nome completo do responsável legal, CPF do responsável legal, E-mail, Telefone, CEP, Endereço, Número, Bairro, Cidade, Estado, Complemento, Descrição Pública do Perfil, Razão Social, Nome Fantasia, CNPJ, Ramo de atuação.</param>
        /// <response code="200">Ok, perfil atualizado.</response>
        /// <response code="400">As informações inseridas são inválidas.</response>
        /// <response code="401">Acesso não autorizado, o email já está associado à outra conta.</response>
        /// <response code="404">Usuário não localizado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpPut("atualizarcadastro/empresa")]
        [Authorize(Roles = Roles.Company)]
        public async Task<IActionResult> UpdateUserCompanyAsync(UpdateCompanyDto dto)
        {
            string? userEmail = GetLoggedUserEmailAddress();

            if (userEmail == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Os dados informados são inválidos." });
            }

            ProfileUpdateResult result = await _userService.UpdateUserCompanyAsync(dto, userEmail);

            return result switch
            {
                ProfileUpdateResult.Success => Ok(),
                ProfileUpdateResult.NotFound => NotFound(),
                ProfileUpdateResult.EmailInUse => Unauthorized(),
                _ => BadRequest(new { message = "Os dados informados são inválidos." })
            };
        }


        // Exclusão
        /// <summary>Deleta a conta do usuário logado - Freelancer e Empresa/Confecção</summary>
        /// <response code="204">Ok, usuário deletado.</response>
        /// <response code="400">A informação inserida é inválida.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        [HttpDelete("deletarcadastro")]
        [Authorize(Roles = $"{Roles.Freelancer}, {Roles.Company}")]
        public async Task<IActionResult> DeleteCurrentUserAsync()
        {
            string? userEmail = GetLoggedUserEmailAddress();

            if (userEmail == null)
            {
                return BadRequest();
            }

            bool deleted = await _userService.DeleteCurrentUserAsync(userEmail);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
