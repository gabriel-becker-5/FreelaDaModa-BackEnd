using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using _01_Presentation.Controllers;
using _02_Application.DTOs.Vaga;
using _02_Application.Enums;
using _02_Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _05_Tests
{
    public class ContratarVagaControllerTests
    {
        private sealed class FakeContratarVagaService : IContratarVagaService
        {
            public bool Chamado { get; private set; }
            public ContratarVagaResultadoDto ResultadoARetornar { get; set; } =
                new(ContratarVagaResultado.Sucesso, "Freelancer contratado com sucesso.", 1);

            public Task<ContratarVagaResultadoDto> ContratarAsync(int vagaId, int usuarioLogadoId, int freelancerId)
            {
                Chamado = true;
                return Task.FromResult(ResultadoARetornar);
            }
        }

        private static ContratarVagaController CriarController(
            FakeContratarVagaService servico,
            int? idUsuarioLogado = 1)
        {
            var controller = new ContratarVagaController(servico);

            var identity = idUsuarioLogado.HasValue
                ? new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.NameId, idUsuarioLogado.Value.ToString()) }, "TesteAuth")
                : new ClaimsIdentity();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return controller;
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornar400_QuandoCorpoEhNulo()
        {
            var servico = new FakeContratarVagaService();
            var controller = CriarController(servico);

            IActionResult resultado = await controller.ContratarAsync(1, null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.False(servico.Chamado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornar400_QuandoFreelancerIdEhZeroOuNegativo()
        {
            var servico = new FakeContratarVagaService();
            var controller = CriarController(servico);

            IActionResult resultado = await controller.ContratarAsync(1, new ContratarVagaDto { FreelancerId = 0 });

            Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.False(servico.Chamado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornar401_QuandoNaoHaClaimDeUsuario()
        {
            var servico = new FakeContratarVagaService();
            var controller = CriarController(servico, idUsuarioLogado: null);

            IActionResult resultado = await controller.ContratarAsync(1, new ContratarVagaDto { FreelancerId = 5 });

            Assert.IsType<UnauthorizedObjectResult>(resultado);
            Assert.False(servico.Chamado);
        }

        [Fact]
        public async Task ContratarAsync_DeveRetornar200_QuandoServicoRetornaSucesso()
        {
            var servico = new FakeContratarVagaService
            {
                ResultadoARetornar = new ContratarVagaResultadoDto(ContratarVagaResultado.Sucesso, "ok", 42)
            };
            var controller = CriarController(servico);

            IActionResult resultado = await controller.ContratarAsync(1, new ContratarVagaDto { FreelancerId = 5 });

            var ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, ok.StatusCode);
            Assert.True(servico.Chamado);
        }

        [Theory]
        [InlineData(ContratarVagaResultado.UsuarioInativo, 401)]
        [InlineData(ContratarVagaResultado.VagaNaoEncontrada, 404)]
        [InlineData(ContratarVagaResultado.NaoAutorizado, 403)]
        [InlineData(ContratarVagaResultado.VagaEncerrada, 409)]
        [InlineData(ContratarVagaResultado.CandidaturaJaAceita, 409)]
        [InlineData(ContratarVagaResultado.FreelancerInvalido, 400)]
        [InlineData(ContratarVagaResultado.CandidaturaNaoEncontrada, 404)]
        [InlineData(ContratarVagaResultado.CandidaturaRejeitada, 409)]
        [InlineData(ContratarVagaResultado.PrazoNoPassado, 400)]
        public async Task ContratarAsync_DeveMapearCadaResultadoParaOCodigoHttpCorreto(
            ContratarVagaResultado resultadoServico, int codigoHttpEsperado)
        {
            var servico = new FakeContratarVagaService
            {
                ResultadoARetornar = new ContratarVagaResultadoDto(resultadoServico, "mensagem qualquer", null)
            };
            var controller = CriarController(servico);

            IActionResult resultado = await controller.ContratarAsync(1, new ContratarVagaDto { FreelancerId = 5 });

            var objectResult = Assert.IsAssignableFrom<ObjectResult>(resultado);
            Assert.Equal(codigoHttpEsperado, objectResult.StatusCode);
        }
    }
}
