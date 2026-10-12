using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using _01_Presentation.Controllers;
using _02_Application.DTOs.Dashboard;
using _02_Application.Services.Dashboard;
using _03_Infrastructure.Data;
using _03_Infrastructure.Repositories;
using _03_Infrastructure.Repositories.Vaga;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _05_Tests
{
    public class DashboardTests
    {
        private static AppDbContext CriarContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private static ObterDashboardFreelancerUseCase CriarFreelancerUseCase(AppDbContext context)
        {
            return new ObterDashboardFreelancerUseCase(
                new UserRepository(context),
                new VagaRepository(context),
                new OrdemServicoRepository(context),
                new CandidaturaRepository(context),
                new AvaliacaoRepository(context),
                new MensagemRepository(context));
        }

        private static ObterDashboardEmpresaUseCase CriarEmpresaUseCase(AppDbContext context)
        {
            return new ObterDashboardEmpresaUseCase(
                new UserRepository(context),
                new VagaRepository(context),
                new OrdemServicoRepository(context),
                new CandidaturaRepository(context),
                new MensagemRepository(context));
        }

        private static DashboardController CriarController(AppDbContext context, int? usuarioId = 1)
        {
            var identity = usuarioId.HasValue
                ? new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, usuarioId.Value.ToString())
                }, "TesteAuth")
                : new ClaimsIdentity();

            return new DashboardController(CriarFreelancerUseCase(context), CriarEmpresaUseCase(context))
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
                }
            };
        }

        private static async Task CriarUsuarioAsync(AppDbContext context, int id = 1, bool excluido = false)
        {
            context.Users.Add(new User
            {
                Id = id,
                LegalResponsibleFullName = "Freelancer de teste",
                LegalResponsibleDocument = "12345678900",
                Email = $"usuario{id}@teste.com",
                PasswordHash = "hash-de-teste",
                ContactNumber = "11999999999",
                PublicProfileDescription = "Perfil de teste",
                PostalCode = "01001000",
                Address = "Rua de teste",
                Neighborhood = "Centro",
                City = "Sao Paulo",
                State = "SP",
                Roles = new List<Roles> { Roles.Freelancer },
                IsDeleted = excluido
            });
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Vagas_DeveRetornarDuasRecentesElegiveis_ComPrazoEmAmbosDashboards()
        {
            using var context = CriarContexto();
            await CriarUsuarioAsync(context);
            var publicacao = new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            for (int id = 1; id <= 6; id++)
            {
                context.Vagas.Add(new Vaga
                {
                    Id = id,
                    UsuarioId = 2,
                    Titulo = $"Vaga {id}",
                    DataPublicacao = publicacao.AddDays(id),
                    PrazoConclusao = publicacao.AddDays(20 + id),
                    Status = id == 5 ? StatusVaga.Pausada
                        : id == 6 ? StatusVaga.Encerrada : StatusVaga.Aberta,
                    IsDeleted = id == 4
                });
            }
            await context.SaveChangesAsync();

            var freelancer = await CriarFreelancerUseCase(context).ExecutarAsync(1);
            var empresa = await CriarEmpresaUseCase(context).ExecutarAsync(2);

            Assert.NotNull(freelancer);
            Assert.Equal(new[] { 3, 2 }, freelancer.VagasRecomendadas.Select(v => v.Id));
            foreach (var vaga in freelancer.VagasRecomendadas)
            {
                Assert.Equal(publicacao.AddDays(20 + vaga.Id), vaga.PrazoConclusao);
            }
            Assert.Equal(new[] { 5, 3, 2, 1 }, empresa.MinhasVagas.Select(v => v.Id));
            foreach (var vaga in empresa.MinhasVagas)
            {
                Assert.Equal(publicacao.AddDays(20 + vaga.Id), vaga.PrazoConclusao);
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task Freelancer_DeveRetornar401_ParaContaAusenteOuExcluida(
            bool criarContaExcluida, bool limparTracking)
        {
            using var context = CriarContexto();
            if (criarContaExcluida)
            {
                await CriarUsuarioAsync(context, excluido: true);
            }
            if (limparTracking)
            {
                context.ChangeTracker.Clear();
            }

            Assert.Null(await CriarFreelancerUseCase(context).ExecutarAsync(1));
            var resultado = await CriarController(context).GetFreelancerDashboardAsync();
            Assert.IsType<UnauthorizedObjectResult>(resultado);
        }

        [Fact]
        public async Task Freelancer_DeveRetornar401_QuandoNaoHaClaimDeUsuario()
        {
            using var context = CriarContexto();
            var resultado = await CriarController(context, usuarioId: null).GetFreelancerDashboardAsync();
            Assert.IsType<UnauthorizedObjectResult>(resultado);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(" EM ANDAMENTO ")]
        public async Task Producoes_DeveNormalizarStatusLegado_SemIncluirConcluidasOuCanceladas(string? status)
        {
            using var context = CriarContexto();
            await CriarUsuarioAsync(context);
            context.OrdensServico.AddRange(
                new OrdemServico { Id = 1, FreelancerId = 1, UserId = 2, Status = status, Valor = 10 },
                new OrdemServico { Id = 2, FreelancerId = 1, UserId = 2, Status = "Concluida", Valor = 20 },
                new OrdemServico { Id = 3, FreelancerId = 1, UserId = 2, Status = "Cancelada", Valor = 30 },
                new OrdemServico { Id = 4, FreelancerId = 1, UserId = 2, Status = "Concluida", Valor = 40, IsDeleted = true });
            await context.SaveChangesAsync();

            var resultado = await CriarFreelancerUseCase(context).ExecutarAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.ProducoesAtivas);
            var producao = Assert.Single(resultado.Producoes);
            Assert.Equal(1, producao.Id);
            Assert.Equal(StatusOrdemServico.EmAndamento.ParaTexto(), producao.Status);
            Assert.Equal(20m, resultado.FaturamentoTotal);
        }

        [Fact]
        public async Task Freelancer_DeveRetornar200ComDadosVazios_ParaContaAtivaSemMovimento()
        {
            using var context = CriarContexto();
            await CriarUsuarioAsync(context);

            var resposta = Assert.IsType<OkObjectResult>(
                await CriarController(context).GetFreelancerDashboardAsync());
            var resultado = Assert.IsType<FreelancerDashboardResponseDto>(resposta.Value);

            Assert.Equal("Freelancer de teste", resultado.NomeExibicao);
            Assert.Null(resultado.MediaAvaliacao);
            Assert.Equal(0, resultado.TotalAvaliacoes);
            Assert.Equal(0, resultado.ProducoesAtivas);
            Assert.Equal(0, resultado.CandidaturasEnviadas);
            Assert.Equal(0m, resultado.FaturamentoTotal);
            Assert.Empty(resultado.VagasRecomendadas);
            Assert.Empty(resultado.Producoes);
            Assert.Empty(resultado.ConversasRecentes);
        }
    }
}
