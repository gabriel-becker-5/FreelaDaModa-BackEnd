using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _01_Presentation.Controllers;
using _02_Application.Services;
using _03_Infrastructure.Data;
using _03_Infrastructure.Repositories;
using _04_Domain.Entities;
using _04_Domain.Enums;

namespace _05_Tests
{
    public class CandidaturaControllerTests
    {
        private static AppDbContext CriarContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private static UserController CriarController(AppDbContext context, int usuarioLogadoId)
        {
            var candidaturaService = new CandidaturaService(new CandidaturaRepository(context));
            var controller = new UserController(null!, candidaturaService, null!);

            var claims = new List<Claim>
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.NameId, usuarioLogadoId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            return controller;
        }

        private static async Task<Vaga> CriarVagaAsync(
            AppDbContext context,
            int usuarioId,
            StatusVaga status = StatusVaga.Aberta,
            bool isDeleted = false)
        {
            var vaga = new Vaga
            {
                Titulo = "Vaga de teste",
                Descricao = "Descricao de teste",
                Orcamento = 100,
                DataPublicacao = DateTime.UtcNow,
                Ativa = true,
                Status = status,
                IsDeleted = isDeleted,
                UsuarioId = usuarioId
            };

            context.Vagas.Add(vaga);
            await context.SaveChangesAsync();
            return vaga;
        }

        private static async Task<StatusCandidatura> ObterStatusAtualAsync(
            AppDbContext context, int vagaId, int freelancerId)
        {
            return await context.Candidaturas
                .AsNoTracking()
                .Where(c => c.VagaId == vagaId && c.FreelancerId == freelancerId)
                .Select(c => c.Status)
                .FirstAsync();
        }

        // --- #11: CriarCandidaturaAsync ---

        [Fact]
        public async Task CriarCandidatura_VagaAberta_Retorna201()
        {
            using var context = CriarContexto();
            var vaga = await CriarVagaAsync(context, usuarioId: 1, status: StatusVaga.Aberta);
            var controller = CriarController(context, usuarioLogadoId: 2);

            var resultado = await controller.CriarCandidaturaAsync(
                new CriarCandidaturaDto { VagaId = vaga.Id, Mensagem = "Olá" });

            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(201, objectResult.StatusCode);
        }

        [Fact]
        public async Task CriarCandidatura_VagaPausada_Retorna409()
        {
            using var context = CriarContexto();
            var vaga = await CriarVagaAsync(context, usuarioId: 1, status: StatusVaga.Pausada);
            var controller = CriarController(context, usuarioLogadoId: 2);

            var resultado = await controller.CriarCandidaturaAsync(
                new CriarCandidaturaDto { VagaId = vaga.Id, Mensagem = "Olá" });

            var conflictResult = Assert.IsType<ConflictObjectResult>(resultado);
            Assert.Equal(409, conflictResult.StatusCode);
            Assert.Equal(0, await context.Candidaturas.CountAsync());
        }

        [Fact]
        public async Task CriarCandidatura_VagaEncerrada_Retorna409()
        {
            using var context = CriarContexto();
            var vaga = await CriarVagaAsync(context, usuarioId: 1, status: StatusVaga.Encerrada);
            var controller = CriarController(context, usuarioLogadoId: 2);

            var resultado = await controller.CriarCandidaturaAsync(
                new CriarCandidaturaDto { VagaId = vaga.Id, Mensagem = "Olá" });

            var conflictResult = Assert.IsType<ConflictObjectResult>(resultado);
            Assert.Equal(409, conflictResult.StatusCode);
            Assert.Equal(0, await context.Candidaturas.CountAsync());
        }

        [Fact]
        public async Task CriarCandidatura_VagaExcluida_Retorna404()
        {
            using var context = CriarContexto();
            var vaga = await CriarVagaAsync(context, usuarioId: 1, status: StatusVaga.Aberta, isDeleted: true);
            var controller = CriarController(context, usuarioLogadoId: 2);

            var resultado = await controller.CriarCandidaturaAsync(
                new CriarCandidaturaDto { VagaId = vaga.Id, Mensagem = "Olá" });

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(0, await context.Candidaturas.CountAsync());
        }

        [Fact]
        public async Task CriarCandidatura_VagaInexistente_Retorna404()
        {
            using var context = CriarContexto();
            var controller = CriarController(context, usuarioLogadoId: 2);

            var resultado = await controller.CriarCandidaturaAsync(
                new CriarCandidaturaDto { VagaId = 999, Mensagem = "Olá" });

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(0, await context.Candidaturas.CountAsync());
        }

        [Fact]
        public async Task CriarCandidatura_Duplicada_Retorna409()
        {
            using var context = CriarContexto();
            var vaga = await CriarVagaAsync(context, usuarioId: 1, status: StatusVaga.Aberta);

            context.Candidaturas.Add(new Candidatura
            {
                VagaId = vaga.Id,
                FreelancerId = 2,
                UsuarioId = 2,
                DataCandidatura = DateTime.UtcNow,
                Status = StatusCandidatura.Pendente
            });
            await context.SaveChangesAsync();

            var controller = CriarController(context, usuarioLogadoId: 2);

            var resultado = await controller.CriarCandidaturaAsync(
                new CriarCandidaturaDto { VagaId = vaga.Id, Mensagem = "Olá" });

            var conflictResult = Assert.IsType<ConflictObjectResult>(resultado);
            Assert.Equal(409, conflictResult.StatusCode);
            Assert.Equal(1, await context.Candidaturas.CountAsync());
        }

        // --- #12: UpdateCandidaturaStatusAsync ---

        private static async Task<(Vaga vaga, Candidatura candidatura)> CriarVagaComCandidaturaAsync(
            AppDbContext context,
            int empresaId,
            StatusVaga status,
            bool vagaExcluida = false)
        {
            var vaga = await CriarVagaAsync(context, usuarioId: empresaId, status: status, isDeleted: vagaExcluida);

            var candidatura = new Candidatura
            {
                VagaId = vaga.Id,
                FreelancerId = 2,
                UsuarioId = 2,
                DataCandidatura = DateTime.UtcNow,
                Status = StatusCandidatura.Pendente
            };
            context.Candidaturas.Add(candidatura);
            await context.SaveChangesAsync();

            return (vaga, candidatura);
        }

        [Fact]
        public async Task UpdateCandidaturaStatus_VagaAberta_Retorna200()
        {
            using var context = CriarContexto();
            var (vaga, candidatura) = await CriarVagaComCandidaturaAsync(context, empresaId: 1, status: StatusVaga.Aberta);
            var controller = CriarController(context, usuarioLogadoId: 1);

            var resultado = await controller.UpdateCandidaturaStatusAsync(
                vaga.Id, candidatura.FreelancerId, StatusCandidatura.Aceita);

            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(
                StatusCandidatura.Aceita,
                await ObterStatusAtualAsync(context, vaga.Id, candidatura.FreelancerId));
        }

        [Fact]
        public async Task UpdateCandidaturaStatus_VagaPausada_Retorna200()
        {
            using var context = CriarContexto();
            var (vaga, candidatura) = await CriarVagaComCandidaturaAsync(context, empresaId: 1, status: StatusVaga.Pausada);
            var controller = CriarController(context, usuarioLogadoId: 1);

            var resultado = await controller.UpdateCandidaturaStatusAsync(
                vaga.Id, candidatura.FreelancerId, StatusCandidatura.Aceita);

            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(
                StatusCandidatura.Aceita,
                await ObterStatusAtualAsync(context, vaga.Id, candidatura.FreelancerId));
        }

        [Fact]
        public async Task UpdateCandidaturaStatus_VagaEncerrada_Retorna404()
        {
            using var context = CriarContexto();
            var (vaga, candidatura) = await CriarVagaComCandidaturaAsync(context, empresaId: 1, status: StatusVaga.Encerrada);
            var controller = CriarController(context, usuarioLogadoId: 1);

            var resultado = await controller.UpdateCandidaturaStatusAsync(
                vaga.Id, candidatura.FreelancerId, StatusCandidatura.Aceita);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(
                StatusCandidatura.Pendente,
                await ObterStatusAtualAsync(context, vaga.Id, candidatura.FreelancerId));
        }

        [Fact]
        public async Task UpdateCandidaturaStatus_VagaExcluida_Retorna404()
        {
            using var context = CriarContexto();
            var (vaga, candidatura) = await CriarVagaComCandidaturaAsync(
                context, empresaId: 1, status: StatusVaga.Aberta, vagaExcluida: true);
            var controller = CriarController(context, usuarioLogadoId: 1);

            var resultado = await controller.UpdateCandidaturaStatusAsync(
                vaga.Id, candidatura.FreelancerId, StatusCandidatura.Aceita);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(
                StatusCandidatura.Pendente,
                await ObterStatusAtualAsync(context, vaga.Id, candidatura.FreelancerId));
        }

        [Fact]
        public async Task UpdateCandidaturaStatus_OutraEmpresa_VagaAberta_Retorna403()
        {
            using var context = CriarContexto();
            var (vaga, candidatura) = await CriarVagaComCandidaturaAsync(context, empresaId: 1, status: StatusVaga.Aberta);
            var controller = CriarController(context, usuarioLogadoId: 99);

            var resultado = await controller.UpdateCandidaturaStatusAsync(
                vaga.Id, candidatura.FreelancerId, StatusCandidatura.Aceita);

            var statusResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(403, statusResult.StatusCode);
            Assert.Equal(
                StatusCandidatura.Pendente,
                await ObterStatusAtualAsync(context, vaga.Id, candidatura.FreelancerId));
        }

        [Fact]
        public async Task UpdateCandidaturaStatus_OutraEmpresa_VagaEncerrada_Retorna403()
        {
            using var context = CriarContexto();
            var (vaga, candidatura) = await CriarVagaComCandidaturaAsync(context, empresaId: 1, status: StatusVaga.Encerrada);
            var controller = CriarController(context, usuarioLogadoId: 99);

            var resultado = await controller.UpdateCandidaturaStatusAsync(
                vaga.Id, candidatura.FreelancerId, StatusCandidatura.Aceita);

            var statusResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(403, statusResult.StatusCode);
            Assert.Equal(
                StatusCandidatura.Pendente,
                await ObterStatusAtualAsync(context, vaga.Id, candidatura.FreelancerId));
        }

        [Fact]
        public async Task UpdateCandidaturaStatus_OutraEmpresa_VagaExcluida_Retorna403()
        {
            using var context = CriarContexto();
            var (vaga, candidatura) = await CriarVagaComCandidaturaAsync(
                context, empresaId: 1, status: StatusVaga.Aberta, vagaExcluida: true);
            var controller = CriarController(context, usuarioLogadoId: 99);

            var resultado = await controller.UpdateCandidaturaStatusAsync(
                vaga.Id, candidatura.FreelancerId, StatusCandidatura.Aceita);

            var statusResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(403, statusResult.StatusCode);
            Assert.Equal(
                StatusCandidatura.Pendente,
                await ObterStatusAtualAsync(context, vaga.Id, candidatura.FreelancerId));
        }
    }
}