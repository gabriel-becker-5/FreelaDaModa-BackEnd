using _02_Application.DTOs.Vaga;
using _02_Application.Interfaces;
using _02_Application.Mappings;
using _03_Infrastructure.Data;
using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace _01_Presentation.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/vagas")]
    [ApiController]
    [Authorize]
    public class VagasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IVagaService _vagaService;

        public VagasController(AppDbContext context, IVagaService vagaService)
        {
            _context = context;
            _vagaService = vagaService;
        }

        // Método auxiliar para buscar o ID do usuário logado de forma segura
        private async Task<User?> GetLoggedUserAsync()
        {
            string? claim = User.FindFirstValue(JwtRegisteredClaimNames.NameId);

            if (!int.TryParse(claim, out int usuarioId) || usuarioId <= 0)
            {
                return null;
            }

            return await _context.Users.FirstOrDefaultAsync(u => u.Id == usuarioId);
        }

        // POST /vagas
        [HttpPost]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> CreateVagaAsync([FromBody] RequisicaoRegistrarVagaJson dto)
        {
            var usuario = await GetLoggedUserAsync();
            if (usuario == null)
            {
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." });
            }

            if (dto.Orcamento < 0.01m || dto.Orcamento > 9999999999999999.99m)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O orçamento deve ser maior que zero e respeitar o limite permitido."
                });
            }

            if (decimal.Round(dto.Orcamento, 2) != dto.Orcamento)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O orçamento deve ter no máximo duas casas decimais."
                });
            }

            DateTime prazoMinimo = new DateTime(1000, 1, 1);
            DateTime prazoMaximo = new DateTime(9999, 12, 31, 23, 59, 59)
                .AddTicks(4999990);

            if (!dto.PrazoConclusao.HasValue ||
                dto.PrazoConclusao.Value < prazoMinimo ||
                dto.PrazoConclusao.Value > prazoMaximo)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "Informe um prazo de conclusão válido."
                });
            }

            try
            {
                var resposta = await _vagaService.RegistrarAsync(dto, usuario.Id);
                return Ok(new { sucesso = true, dados = resposta });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { sucesso = false, mensagem = ex.Message });
            }
        }

        // GET /vagas?empresaId=&usuarioId=&status=
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetVagasAsync(
            [FromQuery] int? empresaId,
            [FromQuery] int? usuarioId,
            [FromQuery] StatusVaga? status)
        {
            var idFiltro = empresaId ?? usuarioId;
            var query = _context.Vagas.Where(v => !v.IsDeleted).AsQueryable();

            if (idFiltro.HasValue)
            {
                query = query.Where(v => v.UsuarioId == idFiltro.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(v => v.Status == status.Value);
            }

            var vagas = await query.ToListAsync();
            return Ok(new { sucesso = true, dados = vagas.ParaRespostaJson() });
        }

        // GET /vagas/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVagaByIdAsync(int id)
        {
            var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

            if (vaga == null)
            {
                return NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." });
            }

            return Ok(new { sucesso = true, dados = vaga.ParaRespostaJson() });
        }

        // PATCH /vagas/{id} (editar)
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateVagaAsync(int id, [FromBody] UpdateVagaDto dto)
        {
            var usuario = await GetLoggedUserAsync();
            if (usuario == null)
            {
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." });
            }

            var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

            if (vaga == null)
            {
                return NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." });
            }

            // CORREÇÃO: Validação de Propriedade (Ownership Check)
            if (vaga.UsuarioId != usuario.Id)
            {
                return StatusCode(403, new { sucesso = false, mensagem = "Você não tem permissão para alterar esta vaga pois ela pertence a outra empresa." });
            }
            if (dto.Titulo != null && string.IsNullOrWhiteSpace(dto.Titulo))
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O título não pode estar vazio."
                });
            }

            if (dto.Descricao != null && string.IsNullOrWhiteSpace(dto.Descricao))
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "A descrição não pode estar vazia."
                });
            }

            if (dto.Cidade != null && string.IsNullOrWhiteSpace(dto.Cidade))
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "A cidade não pode estar vazia."
                });
            }

            if (dto.Estado != null && string.IsNullOrWhiteSpace(dto.Estado))
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O estado não pode estar vazio."
                });
            }

            if (dto.Orcamento.HasValue)
            {
                decimal orcamento = dto.Orcamento.Value;

                if (orcamento < 0.01m || orcamento > 9999999999999999.99m)
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        mensagem = "O orçamento deve ser maior que zero e respeitar o limite permitido."
                    });
                }

                if (decimal.Round(orcamento, 2) != orcamento)
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        mensagem = "O orçamento deve ter no máximo duas casas decimais."
                    });
                }
            }

            if (dto.PrazoConclusao.HasValue)
            {
                DateTime prazoMinimo = new DateTime(1000, 1, 1);
                DateTime prazoMaximo = new DateTime(9999, 12, 31, 23, 59, 59)
                    .AddTicks(4999990);

                if (dto.PrazoConclusao.Value < prazoMinimo ||
                    dto.PrazoConclusao.Value > prazoMaximo)
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        mensagem = "Informe um prazo de conclusão válido."
                    });
                }
            }

            if (dto.Titulo is not null && vaga.Titulo != dto.Titulo)
            {
                vaga.Titulo = dto.Titulo;
            }

            if (dto.Descricao is not null && vaga.Descricao != dto.Descricao)
            {
                vaga.Descricao = dto.Descricao;
            }

            if (dto.Especialidade is not null && vaga.Especialidade != dto.Especialidade)
            {
                vaga.Especialidade = (Specialty)dto.Especialidade;
            }

            if (dto.Modalidade is not null && vaga.Modalidade != dto.Modalidade)
            {
                vaga.Modalidade = (ModalidadeVaga)dto.Modalidade;
            }

            if (dto.Cidade is not null && vaga.Cidade != dto.Cidade)
            {
                vaga.Cidade = dto.Cidade;
            }

            if (dto.Estado is not null && vaga.Estado != dto.Estado)
            {
                vaga.Estado = dto.Estado;
            }

            if (dto.Orcamento is not null && vaga.Orcamento != dto.Orcamento)
            {
                vaga.Orcamento = (decimal)dto.Orcamento;
            }

            if (dto.PrazoConclusao is not null && vaga.PrazoConclusao != dto.PrazoConclusao)
            {
                vaga.PrazoConclusao = (DateTime)dto.PrazoConclusao;
            }

            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true, mensagem = "Vaga atualizada com sucesso.", dados = vaga.ParaRespostaJson() });
        }

        // PATCH /vagas/{id}/status (encerrar/pausar)
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateVagaStatusAsync(int id, [FromBody] UpdateVagaStatusDto dto)
        {
            var usuario = await GetLoggedUserAsync();
            if (usuario == null)
            {
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." });
            }

            var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

            if (vaga == null)
            {
                return NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." });
            }

            // CORREÇÃO: Validação de Propriedade (Ownership Check)
            if (vaga.UsuarioId != usuario.Id)
            {
                return StatusCode(403, new { sucesso = false, mensagem = "Você não tem permissão para alterar o status desta vaga." });
            }

            if (!dto.Status.HasValue)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "O status da vaga é obrigatório."
                });
            }

            vaga.Status = dto.Status.Value;
            vaga.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true, mensagem = "Status da vaga atualizado com sucesso.", dados = vaga.ParaRespostaJson() });
        }

        // DELETE /vagas/{id} (soft delete, só empresa)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> DeleteVagaAsync(int id)
        {
            var usuario = await GetLoggedUserAsync();
            if (usuario == null)
            {
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não encontrado." });
            }

            var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

            if (vaga == null)
            {
                return NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." });
            }

            // CORREÇÃO: Validação de Propriedade (Ownership Check)
            if (vaga.UsuarioId != usuario.Id)
            {
                return StatusCode(403, new { sucesso = false, mensagem = "Você não tem permissão para excluir esta vaga." });
            }

            vaga.IsDeleted = true;
            vaga.UpdatedAt = DateTime.UtcNow;

            _context.Vagas.Update(vaga);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
