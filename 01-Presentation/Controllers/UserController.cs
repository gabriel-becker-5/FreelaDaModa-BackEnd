using _01_Presentation.Requests;
using _02_Application.DTOs;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.ProfileImage;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _04_Domain.Enums;
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
        private readonly IProfileImageService _profileImageService;

        public UserController(IUserService userservice, IProfileImageService profileimageservice)
        {
            _userService = userservice;
            _profileImageService = profileimageservice;
        }

        private int? GetLoggedUserId()
        {
            string? userId = User.FindFirstValue(JwtRegisteredClaimNames.NameId);
            return int.TryParse(userId, out int id) ? id : null;
        }

        // Criação
        /// <summary>Cria um novo usuário do tipo Freelancer</summary>
        /// <param name="dto">Campos: Nome completo do responsável legal, CPF do responsável legal, E-mail, Senha, Telefone, CEP, Endereço, Número, Bairro, Cidade, Estado, Complemento, Descrição Pública do Perfil, Data de nascimento, Tempo de experiência, Especialidades, Máquinas que possui, Disponibilidade de tempo.</param>
        /// <returns>Conta/perfil do usuário criada.</returns>
        /// <response code="201">Conta/perfil criada com sucesso.</response>
        /// <response code="400">Informações inseridas inválidas.</response>
        /// <response code="409">E-mail ou CPF já associado à outra conta.</response>
        /// <response code="500">Falha ao criar a conta.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPost("cadastrar/freelancer")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateFreelancerAsync(CreateFreelancerDto dto)
        {
            CreateUserResult result = await _userService.CreateFreelancerAsync(dto);

            return result.Status switch
            {
                CreateUserStatus.Success => CreatedAtAction(null, null),
                CreateUserStatus.EmailInUse => Conflict(new { message = "O e-mail informado já está em uso por outra conta." }),
                CreateUserStatus.CpfInUse => Conflict(new { message = "O CPF informado já está em uso por outra conta." }),
                CreateUserStatus.InvalidCPF => BadRequest(new { message = "O CPF informado é inválido." }),
                CreateUserStatus.InvalidData => BadRequest(new { message = "Os dados informados são inválidos.", errors = result.Errors }),
                _ => StatusCode(500, new { message = "Não foi possível criar a conta. Tente novamente." })
            };
        }

        /// <summary>Cria um novo usuário do tipo Empresa/Confecção</summary>
        /// <param name="dto">Campos: Nome completo do responsável legal, Data de nascimento, CPF do responsável legal, E-mail, Senha, Telefone, CEP, Endereço, Número, Bairro, Cidade, Estado, Complemento, Descrição Pública do Perfil, Razão Social, Nome Fantasia, CNPJ, Ramo de atuação.</param>
        /// <returns>Conta/perfil do usuário criada.</returns>
        /// <response code="201">Conta/perfil criada com sucesso.</response>
        /// <response code="400">Informações inseridas inválidas.</response>
        /// <response code="409">E-mail, CPF ou CNPJ já associado à outra conta.</response>
        /// <response code="500">Falha ao criar a conta.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPost("cadastrar/empresa")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCompanyAsync(CreateCompanyDto dto)
        {
            CreateUserResult result = await _userService.CreateCompanyAsync(dto);

            return result.Status switch
            {
                CreateUserStatus.Success => CreatedAtAction(null, null),
                CreateUserStatus.EmailInUse => Conflict(new { message = "O e-mail informado já está em uso por outra conta." }),
                CreateUserStatus.CpfInUse => Conflict(new { message = "O CPF informado já está em uso por outra conta." }),
                CreateUserStatus.CnpjInUse => Conflict(new { message = "O CNPJ informado já está em uso por outra conta." }),
                CreateUserStatus.InvalidCPF => BadRequest(new { message = "O CPF informado é inválido." }),
                CreateUserStatus.InvalidCNPJ => BadRequest(new { message = "O CNPJ informado é inválido." }),
                CreateUserStatus.InvalidData => BadRequest(new { message = "Os dados informados são inválidos.", errors = result.Errors }),
                _ => StatusCode(500, new { message = "Não foi possível criar a conta. Tente novamente." })
            };
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
        [Authorize(Roles = nameof(Roles.Freelancer))]
        public async Task<IActionResult> GetUserFreelancerAsync()
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return BadRequest();
            }

            GetFreelancerDto? dtoLoggedUser = await _userService.GetFreelancerProfileByIdAsync((int)userId);

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
        [Authorize(Roles = nameof(Roles.Company))]
        public async Task<IActionResult> GetUserCompanyAsync()
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return BadRequest();
            }

            GetCompanyDto? dtoLoggedUser = await _userService.GetCompanyProfileByIdAsync((int)userId);

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
        /// <response code="409">O e-mail informado já está associado à outra conta.</response>
        /// <response code="404">Usuário não localizado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(404)]
        [HttpPatch("atualizarcadastro/freelancer")]
        [Authorize(Roles = nameof(Roles.Freelancer))]
        public async Task<IActionResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto)
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return BadRequest();
            }

            ProfileUpdateResult result = await _userService.UpdateUserFreelancerAsync(dto, (int)userId);

            return result switch
            {
                ProfileUpdateResult.Success => Ok(),
                ProfileUpdateResult.NotFound => NotFound(),
                ProfileUpdateResult.EmailInUse => Conflict(new { message = "O e-mail informado já está em uso por outra conta." }),
                ProfileUpdateResult.DocumentInUse => Conflict(new { message = "O documento informado (CPF/CNPJ) já está em uso por outra conta." }),
                _ => BadRequest(new { message = "Os dados informados são inválidos." })
            };
        }

        /// <summary>Atualiza o perfil do usuário logado - Empresa/Confecção</summary>
        /// <param name="dto">Campos: Nome completo do responsável legal, CPF do responsável legal, E-mail, Telefone, CEP, Endereço, Número, Bairro, Cidade, Estado, Complemento, Descrição Pública do Perfil, Razão Social, Nome Fantasia, CNPJ, Ramo de atuação.</param>
        /// <response code="200">Ok, perfil atualizado.</response>
        /// <response code="400">As informações inseridas são inválidas.</response>
        /// <response code="409">O e-mail informado já está associado à outra conta.</response>
        /// <response code="404">Usuário não localizado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(404)]
        [HttpPatch("atualizarcadastro/empresa")]
        [Authorize(Roles = nameof(Roles.Company))]
        public async Task<IActionResult> UpdateUserCompanyAsync(UpdateCompanyDto dto)
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return BadRequest(new { message = "Os dados informados são inválidos." });
            }

            ProfileUpdateResult result = await _userService.UpdateUserCompanyAsync(dto, (int)userId);

            return result switch
            {
                ProfileUpdateResult.Success => Ok(),
                ProfileUpdateResult.NotFound => NotFound(),
                ProfileUpdateResult.EmailInUse => Conflict(new { message = "O e-mail informado já está em uso por outra conta." }),
                ProfileUpdateResult.DocumentInUse => Conflict(new { message = "O documento informado (CPF/CNPJ) já está em uso por outra conta." }),
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
        [Authorize(Roles = $"{nameof(Roles.Freelancer)}, {nameof(Roles.Company)}")]
        public async Task<IActionResult> DeleteCurrentUserAsync()
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return BadRequest();
            }

            bool deleted = await _userService.DeleteUserByIdAsync((int)userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }


        /// <summary>Envia ou substitui a foto de perfil do usuário logado (PNG/JPG até 5 MiB).</summary>
        /// <response code="200">Upload concluído; retorna profileImageUrl.</response>
        /// <response code="404">Usuário não localizado.</response>
        /// <response code="413">Arquivo excede 5 MiB.</response>
        /// <response code="415">Formato não permitido (apenas PNG/JPG).</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(413)]
        [ProducesResponseType(415)]
        [HttpPut("perfil/imagem")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
        [Authorize(Roles = $"{nameof(Roles.Freelancer)}, {nameof(Roles.Company)}")]
        public async Task<IActionResult> UploadProfileImageAsync(
            [FromForm] UploadProfileImageRequest request,
            CancellationToken cancellationToken)
        {
            int? userId = GetLoggedUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            await using Stream content = request.Image.OpenReadStream();

            ProfileImageUpdateResult result = await _profileImageService.ReplaceAsync(
                (int)userId,
                new ProfileImageUpload(content, request.Image.FileName, request.Image.ContentType, request.Image.Length),
                cancellationToken);

            return result switch
            {
                ProfileImageUpdateResult.Success => Ok(new { profileImageUrl = $"/api/v1/User/{userId}/imagem-perfil" }),
                ProfileImageUpdateResult.UserNotFound => NotFound(),
                ProfileImageUpdateResult.EmptyFile => BadRequest(new { message = "O arquivo de imagem está vazio." }),
                ProfileImageUpdateResult.FileTooLarge => StatusCode(413, new { message = "A imagem excede o limite de 5 MB." }),
                ProfileImageUpdateResult.UnsupportedFormat => StatusCode(415, new { message = "Formato não permitido. Envie apenas PNG ou JPG." }),
                ProfileImageUpdateResult.Conflict => Conflict(new { message = "A foto foi alterada por outra requisição. Tente novamente." }),
                _ => StatusCode(500, new { message = "Não foi possível salvar a imagem. Tente novamente." })
            };
        }


        /// <summary>Obtém a foto de perfil atual de um usuário ativo.</summary>
        /// <response code="200">Ok, retorna a imagem.</response>
        /// <response code="404">Usuário inexistente/excluído ou sem foto.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        [HttpGet("{userId:int}/imagem-perfil")]
        public async Task<IActionResult> GetProfileImageAsync(int userId, CancellationToken cancellationToken)
        {
            ProfileImageReadResult? image = await _profileImageService.GetAsync(userId, cancellationToken);

            if (image == null)
            {
                return NotFound();
            }

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            Response.Headers["Cache-Control"] = "public, no-cache";
            Response.Headers["ETag"] = $"\"{image.ETag}\"";

            return File(image.Content, image.ContentType);
        }

        /// <summary>Remove a foto de perfil do usuário logado.</summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("perfil/imagem")]
        [Authorize(Roles = $"{nameof(Roles.Freelancer)}, {nameof(Roles.Company)}")]
        public async Task<IActionResult> DeleteProfileImageAsync(CancellationToken cancellationToken)
        {
            int? userId = GetLoggedUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            ProfileImageUpdateResult result = await _profileImageService.RemoveAsync((int)userId, cancellationToken);

            return result switch
            {
                ProfileImageUpdateResult.Success => NoContent(),
                ProfileImageUpdateResult.UserNotFound => NotFound(),
                ProfileImageUpdateResult.Conflict => Conflict(new { message = "A foto foi alterada por outra requisição. Tente novamente." }),
                _ => StatusCode(500, new { message = "Não foi possível remover a imagem. Tente novamente." })
            };
        }

    }
}