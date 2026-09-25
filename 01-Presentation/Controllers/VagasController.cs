using _02_Application.DTOs.Vaga;
using _02_Application.Interfaces;
using _02_Application.Services.Vaga;
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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/vagas")]
    [ApiController]
    [Authorize]
    public class VagasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RegistrarVagaUseCase _registrarVagaUseCase;

        public VagasController(AppDbContext context, RegistrarVagaUseCase registrarVagaUseCase)
        {
            _context = context;
            _registrarVagaUseCase = registrarVagaUseCase;
        }

        private string? GetLoggedUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
        }

        // POST /vagas
        [HttpPost]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> CreateVagaAsync([FromBody] RequisicaoRegistrarVagaJson dto)
        {
            var userEmail = GetLoggedUserEmail();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { sucesso = false, mensagem = "E-mail não encontrado no token JWT." });
            }

            try
            {
                var resposta = await _registrarVagaUseCase.ExecutarAsync(dto, userEmail);
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
            return Ok(new { sucesso = true, dados = vagas });
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

            return Ok(new { sucesso = true, dados = vaga });
        }

        // PATCH /vagas/{id} (editar)
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateVagaAsync(int id, [FromBody] UpdateVagaDto dto)
        {
            var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

            if (vaga == null)
            {
                return NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." });
            }

            vaga.Titulo = dto.Titulo ?? vaga.Titulo;
            vaga.Descricao = dto.Descricao ?? vaga.Descricao;
            vaga.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true, mensagem = "Vaga atualizada com sucesso.", dados = vaga });
        }

        // PATCH /vagas/{id}/status (encerrar/pausar)
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateVagaStatusAsync(int id, [FromBody] UpdateVagaStatusDto dto)
        {
            var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

            if (vaga == null)
            {
                return NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." });
            }

            vaga.Status = dto.Status;
            vaga.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true, mensagem = "Status da vaga atualizado com sucesso.", dados = vaga });
        }

        // DELETE /vagas/{id} (soft delete, só empresa)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> DeleteVagaAsync(int id)
        {
            var vaga = await _context.Vagas.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

            if (vaga == null)
            {
                return NotFound(new { sucesso = false, mensagem = "Vaga não encontrada." });
            }

            vaga.IsDeleted = true;
            vaga.UpdatedAt = DateTime.UtcNow;

            _context.Vagas.Update(vaga);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class UpdateVagaDto
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
    }

    public class UpdateVagaStatusDto
    {
        public StatusVaga Status { get; set; }
    }
}