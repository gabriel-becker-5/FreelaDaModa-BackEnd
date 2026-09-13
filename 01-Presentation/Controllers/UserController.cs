using _02_Application.Authorization;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.Interfaces;
using _04_Domain.Entities.Profiles;
using _04_Domain.Entities.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using _02_Application.DTOs.User;

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
        private readonly IFreelancerFieldsService _freelancerFieldsService;

        public UserController(IUserService userservice,
                              IRoleService roleService,
                              IFreelancerFieldsService freelancerFieldsService)
        {
            _userService = userservice;
            _roleService = roleService;
            _freelancerFieldsService = freelancerFieldsService;
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

            int? roleId = await _roleService.GetRoleIdByNameAsync(Roles.Freelancer);

            if (roleId == null)
            {
                return BadRequest();
            }

            int? newUser = await _userService.CreateFreelancerAsync(dto);

            if (newUser == null)
            {
                return BadRequest();
            }

            await _userService.CreateUserRoleAsync((int)newUser, (int)roleId);

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

            int? roleId = await _roleService.GetRoleIdByNameAsync(Roles.Company);

            if (roleId == null)
            {
                return BadRequest();
            }

            int? newUser = await _userService.CreateCompanyAsync(dto);

            if (newUser == null)
            {
                return BadRequest();
            }

            await _userService.CreateUserRoleAsync((int)newUser, (int)roleId);

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

            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return NotFound();
            }

            FreelancerProfile? loggedProfile = await _userService.GetFreelancerProfileAsync((int)id);

            if (loggedProfile == null)
            {
                return NotFound();
            }

            FreelancerProfileDto? freelancerProfile = await _freelancerFieldsService.GetProfileFieldsNames(loggedProfile.Id);

            if (freelancerProfile == null)
            {
                return NotFound();
            }

            UserDto loggedUser = await _userService.GetUserByEmailAsync(userEmail);

            GetFreelancerDto dtoLoggedUser = new()
            {
                Email = loggedUser.Email,
                PublicProfileDescription = loggedUser.PublicProfileDescription,
                ContactNumber = loggedUser.ContactNumber,
                LegalResponsibleDocument = loggedUser.LegalResponsibleDocument,
                LegalResponsibleFullName = loggedUser.LegalResponsibleFullName,
                Address = loggedUser.Address,
                AddressNumber = loggedUser.AddressNumber,
                Quarter = loggedUser.Quarter,
                City = loggedUser.City,
                State = loggedUser.State,
                AdditionalAddressInfo = loggedUser.AdditionalAddressInfo,
                PostalCode = loggedUser.PostalCode,
                HasFixedProducer = freelancerProfile.HasFixedProducer,
                HasOwnCar = freelancerProfile.HasOwnCar,
                BirthDate = freelancerProfile.BirthDate,
                OwnMachineNames = freelancerProfile.OwnMachines,
                SpecialtyNames = freelancerProfile.Specialties,
                AvailableTimeName = freelancerProfile.AvailableTimeName,
                AverageRevenueName = freelancerProfile.AverageRevenueName,
                HowUsuallyArrangeServicesName = freelancerProfile.HowUsuallyArrangeServicesName,
                BusinessTypeName = freelancerProfile.BusinessTypeName,
                ExperienceYearsName = freelancerProfile.ExperienceYearsName,
                FreelancerPreferencesName = freelancerProfile.FreelancerPreferencesName,
                WorkshopSizeName = freelancerProfile.WorkshopSizeName
            };

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

            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return NotFound();
            }

            CompanyProfile? loggedProfile = await _userService.GetCompanyProfileAsync((int)id);

            if (loggedProfile == null)
            {
                return NotFound();
            }

            UserDto loggedUser = await _userService.GetUserByEmailAsync(userEmail);

            GetCompanyDto dtoLoggedUser = new()
            {
                Email = loggedUser.Email,
                PublicProfileDescription = loggedUser.PublicProfileDescription,
                ContactNumber = loggedUser.ContactNumber,
                LegalResponsibleDocument = loggedUser.LegalResponsibleDocument,
                LegalResponsibleFullName = loggedUser.LegalResponsibleFullName,
                Address = loggedUser.Address,
                AddressNumber = loggedUser.AddressNumber,
                Quarter = loggedUser.Quarter,
                City = loggedUser.City,
                State = loggedUser.State,
                AdditionalAddressInfo = loggedUser.AdditionalAddressInfo,
                PostalCode = loggedUser.PostalCode,
                LegalName = loggedProfile.LegalName,
                CompanyName = loggedProfile.CompanyName,
                CompanyRegistrationDocument = loggedProfile.CompanyRegistrationDocument,
                CoreBusiness = loggedProfile.CoreBusiness
            };

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

            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return NotFound();
            }

            FreelancerProfile? loggedProfile = await _userService.GetFreelancerProfileAsync((int)id);

            if (loggedProfile == null)
            {
                return NotFound();
            }

            UserDto loggedUser = await _userService.GetUserByEmailAsync(userEmail);

            if (dto.Email != loggedUser.Email && await _userService.IsUserEmailRegistered(dto.Email))
            {
                return Unauthorized();
            }

            if (!await _userService.UpdateUserFreelancerAsync(dto, loggedUser, loggedProfile))
            {
                return BadRequest(new { message = "Os dados informados são inválidos." });
            }
            
            return Ok();
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
                return BadRequest();
            }

            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return NotFound();
            }

            UserDto loggedUser = await _userService.GetUserByEmailAsync(userEmail);

            CompanyProfile? loggedProfile = await _userService.GetCompanyProfileAsync((int)id);

            if (loggedProfile == null)
            {
                return NotFound();
            }

            if (dto.Email != loggedUser.Email && await _userService.IsUserEmailRegistered(dto.Email))
            {
                return Unauthorized();
            }

            if (!await _userService.UpdateUserCompanyAsync(dto, loggedUser, loggedProfile))
            {
                return BadRequest(new { message = "Os dados informados são inválidos." });
            }

            return Ok();
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

            int? id = await _userService.GetUserIdByEmailAsync(userEmail);

            if (id == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserByIdAsync((int)id);

            return NoContent();
        }
    }
}