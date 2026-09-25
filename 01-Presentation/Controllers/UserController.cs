using _01_Presentation.Requests;
using _02_Application.DTOs;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.ProfileImage;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _03_Infrastructure.Data;
using _04_Domain.Entities;
using _04_Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;
        private readonly IProfileImageService _profileImageService;

        public UserController(
            IUserService userService,
            AppDbContext context,
            IProfileImageService profileImageService)
        {
            _userService = userService;
            _context = context;
            _profileImageService = profileImageService;
        }

        private int? GetLoggedUserId()
        {
            string? userId =
                User.FindFirstValue(JwtRegisteredClaimNames.NameId);

            if (int.TryParse(userId, out int id))
            {
                return id;
            }

            string? userIdAlternative =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdAlternative, out int alternativeId))
            {
                return alternativeId;
            }

            return null;
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
        public async Task<IActionResult> CreateFreelancerAsync(
            [FromBody] CreateFreelancerDto dto)
        {
            CreateUserResult result =
                await _userService.CreateFreelancerAsync(dto);

            return result.Status switch
            {
                CreateUserStatus.Success =>
                    Created(
                        string.Empty,
                        new
                        {
                            message = "Freelancer criado com sucesso."
                        }),

                CreateUserStatus.EmailInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O e-mail informado já está em uso por outra conta."
                        }),

                CreateUserStatus.CpfInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O CPF informado já está em uso por outra conta."
                        }),

                CreateUserStatus.InvalidCPF =>
                    BadRequest(
                        new
                        {
                            message = "O CPF informado é inválido."
                        }),

                CreateUserStatus.InvalidData =>
                    BadRequest(
                        new
                        {
                            message =
                                "Os dados informados são inválidos.",
                            errors = result.Errors
                        }),

                _ =>
                    StatusCode(
                        500,
                        new
                        {
                            message =
                                "Não foi possível criar a conta. Tente novamente."
                        })
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
        public async Task<IActionResult> CreateCompanyAsync(
            [FromBody] CreateCompanyDto dto)
        {
            CreateUserResult result =
                await _userService.CreateCompanyAsync(dto);

            return result.Status switch
            {
                CreateUserStatus.Success =>
                    Created(
                        string.Empty,
                        new
                        {
                            message = "Empresa criada com sucesso."
                        }),

                CreateUserStatus.EmailInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O e-mail informado já está em uso por outra conta."
                        }),

                CreateUserStatus.CpfInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O CPF informado já está em uso por outra conta."
                        }),

                CreateUserStatus.CnpjInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O CNPJ informado já está em uso por outra conta."
                        }),

                CreateUserStatus.InvalidCPF =>
                    BadRequest(
                        new
                        {
                            message = "O CPF informado é inválido."
                        }),

                CreateUserStatus.InvalidCNPJ =>
                    BadRequest(
                        new
                        {
                            message = "O CNPJ informado é inválido."
                        }),

                CreateUserStatus.InvalidData =>
                    BadRequest(
                        new
                        {
                            message =
                                "Os dados informados são inválidos.",
                            errors = result.Errors
                        }),

                _ =>
                    StatusCode(
                        500,
                        new
                        {
                            message =
                                "Não foi possível criar a conta. Tente novamente."
                        })
            };
        }

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
                return BadRequest(
                    new
                    {
                        message = "Utilizador inválido."
                    });
            }

            GetFreelancerDto? dtoLoggedUser =
                await _userService.GetFreelancerProfileByIdAsync(
                    userId.Value);

            if (dtoLoggedUser == null)
            {
                return NotFound();
            }

            return Ok(dtoLoggedUser);
        }

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
                return BadRequest(
                    new
                    {
                        message = "Utilizador inválido."
                    });
            }

            GetCompanyDto? dtoLoggedUser =
                await _userService.GetCompanyProfileByIdAsync(
                    userId.Value);

            if (dtoLoggedUser == null)
            {
                return NotFound();
            }

            return Ok(dtoLoggedUser);
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(404)]
        [HttpPatch("atualizarcadastro/freelancer")]
        [Authorize(Roles = nameof(Roles.Freelancer))]
        public async Task<IActionResult> UpdateUserFreelancerAsync(
            [FromBody] UpdateFreelancerDto dto)
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return BadRequest(
                    new
                    {
                        message = "Utilizador inválido."
                    });
            }

            ProfileUpdateResult result =
                await _userService.UpdateUserFreelancerAsync(
                    dto,
                    userId.Value);

            return result switch
            {
                ProfileUpdateResult.Success =>
                    Ok(),

                ProfileUpdateResult.NotFound =>
                    NotFound(),

                ProfileUpdateResult.EmailInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O e-mail informado já está em uso por outra conta."
                        }),

                ProfileUpdateResult.DocumentInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O documento informado já está em uso por outra conta."
                        }),

                _ =>
                    BadRequest(
                        new
                        {
                            message =
                                "Os dados informados são inválidos."
                        })
            };
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(404)]
        [HttpPatch("atualizarcadastro/empresa")]
        [Authorize(Roles = nameof(Roles.Company))]
        public async Task<IActionResult> UpdateUserCompanyAsync(
            [FromBody] UpdateCompanyDto dto)
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return BadRequest(
                    new
                    {
                        message = "Utilizador inválido."
                    });
            }

            ProfileUpdateResult result =
                await _userService.UpdateUserCompanyAsync(
                    dto,
                    userId.Value);

            return result switch
            {
                ProfileUpdateResult.Success =>
                    Ok(),

                ProfileUpdateResult.NotFound =>
                    NotFound(),

                ProfileUpdateResult.EmailInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O e-mail informado já está em uso por outra conta."
                        }),

                ProfileUpdateResult.DocumentInUse =>
                    Conflict(
                        new
                        {
                            message =
                                "O documento informado já está em uso por outra conta."
                        }),

                _ =>
                    BadRequest(
                        new
                        {
                            message =
                                "Os dados informados são inválidos."
                        })
            };
        }

        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpDelete("deletarcadastro")]
        [Authorize(Roles =
            $"{nameof(Roles.Freelancer)}, {nameof(Roles.Company)}")]
        public async Task<IActionResult> DeleteCurrentUserAsync()
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return BadRequest(
                    new
                    {
                        message = "Utilizador inválido."
                    });
            }

            bool deleted =
                await _userService.DeleteUserByIdAsync(
                    userId.Value);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpPost("candidaturas")]
        [Authorize(Roles = nameof(Roles.Freelancer))]
        public async Task<IActionResult> CriarCandidaturaAsync(
            [FromBody] CriarCandidaturaDto dto)
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return Unauthorized(
                    new
                    {
                        sucesso = false,
                        mensagem =
                            "Não foi possível identificar o usuário logado."
                    });
            }

            if (dto.VagaId <= 0)
            {
                return BadRequest(
                    new
                    {
                        sucesso = false,
                        mensagem =
                            "O VagaId deve ser maior que zero."
                    });
            }

            var vaga = await _context.Vagas
                .FirstOrDefaultAsync(v => v.Id == dto.VagaId);

            if (vaga == null)
            {
                return NotFound(
                    new
                    {
                        sucesso = false,
                        mensagem =
                            "A vaga informada não foi encontrada."
                    });
            }

            bool candidaturaExistente =
                await _context.Candidaturas.AnyAsync(c =>
                    c.VagaId == dto.VagaId &&
                    c.FreelancerId == userId.Value);

            if (candidaturaExistente)
            {
                return Conflict(
                    new
                    {
                        sucesso = false,
                        mensagem =
                            "Você já se candidatou a esta vaga."
                    });
            }

            var novaCandidatura = new Candidatura
            {
                VagaId = dto.VagaId,
                FreelancerId = userId.Value,
                UsuarioId = userId.Value,
                DataCandidatura = DateTime.UtcNow,
                Mensagem = dto.Mensagem,
                CreatedAt = DateTime.UtcNow,
                Status = StatusCandidatura.Pendente
            };

            _context.Candidaturas.Add(novaCandidatura);

            await _context.SaveChangesAsync();

            return StatusCode(
                201,
                new
                {
                    sucesso = true,
                    mensagem =
                        "Candidatura realizada com sucesso.",
                    dados = new
                    {
                        id = novaCandidatura.Id,
                        vagaId = novaCandidatura.VagaId,
                        freelancerId = novaCandidatura.FreelancerId,
                        usuarioId = novaCandidatura.UsuarioId,
                        dataCandidatura = novaCandidatura.DataCandidatura,
                        mensagem = novaCandidatura.Mensagem,
                        status = novaCandidatura.Status,
                        createdAt = novaCandidatura.CreatedAt
                    }
                });
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [HttpGet("candidaturas/minhas")]
        [Authorize(Roles = nameof(Roles.Freelancer))]
        public async Task<IActionResult> GetMinhasCandidaturasAsync()
        {
            int? userId = GetLoggedUserId();

            if (userId == null)
            {
                return Unauthorized(
                    new
                    {
                        sucesso = false,
                        mensagem =
                            "Não foi possível identificar o usuário logado."
                    });
            }

            var candidaturas =
                await _context.Candidaturas
                    .Include(c => c.Vaga)
                    .Where(c =>
                        c.FreelancerId == userId.Value)
                    .Select(c => new
                    {
                        id = c.Id,
                        vagaId = c.VagaId,
                        tituloVaga =
                            c.Vaga != null
                                ? c.Vaga.Titulo
                                : "Vaga",
                        dataCandidatura = c.DataCandidatura,
                        createdAt = c.CreatedAt,
                        status = c.Status,
                        mensagem = c.Mensagem
                    })
                    .ToListAsync();

            return Ok(
                new
                {
                    sucesso = true,
                    mensagem =
                        "Candidaturas obtidas com sucesso.",
                    total = candidaturas.Count,
                    dados = candidaturas
                });
        }

        // GET /candidaturas?freelancerId=&empresaId=&vagaId=&status=
        [ProducesResponseType(200)]
        [HttpGet("candidaturas")]
        [Authorize]
        public async Task<IActionResult> GetCandidaturasAsync(
            [FromQuery] int? freelancerId,
            [FromQuery] int? empresaId,
            [FromQuery] int? vagaId,
            [FromQuery] StatusCandidatura? status)
        {
            var query = _context.Candidaturas
                .Include(c => c.Vaga)
                .AsQueryable();

            if (freelancerId.HasValue)
                query = query.Where(c => c.FreelancerId == freelancerId.Value);

            if (vagaId.HasValue)
                query = query.Where(c => c.VagaId == vagaId.Value);

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            if (empresaId.HasValue)
                query = query.Where(c => c.Vaga != null && c.Vaga.UsuarioId == empresaId.Value);

            var resultado = await query.Select(c => new
            {
                id = c.Id,
                vagaId = c.VagaId,
                tituloVaga = c.Vaga != null ? c.Vaga.Titulo : "Vaga",
                freelancerId = c.FreelancerId,
                dataCandidatura = c.DataCandidatura,
                createdAt = c.CreatedAt,
                status = c.Status,
                mensagem = c.Mensagem
            }).ToListAsync();

            return Ok(new
            {
                sucesso = true,
                mensagem = "Candidaturas filtradas obtidas com sucesso.",
                total = resultado.Count,
                dados = resultado
            });
        }

        // PATCH /candidaturas/{id}/status (Selecionado/Rejeitado/Cancelada)
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPatch("candidaturas/{id:int}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateCandidaturaStatusAsync(
            int id,
            [FromBody] UpdateCandidaturaStatusDto dto)
        {
            var candidatura = await _context.Candidaturas.FindAsync(id);
            if (candidatura == null)
            {
                return NotFound(new
                {
                    sucesso = false,
                    mensagem = "Candidatura não encontrada."
                });
            }

            candidatura.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                sucesso = true,
                mensagem = "Status da candidatura atualizado com sucesso.",
                dados = candidatura
            });
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

    public class CriarCandidaturaDto
    {
        public int VagaId { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }

    public class UpdateCandidaturaStatusDto
    {
        public StatusCandidatura Status { get; set; }
    }
}
