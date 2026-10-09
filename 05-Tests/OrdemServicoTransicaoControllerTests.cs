using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using _01_Presentation.Controllers;
using _02_Application.DTOs.OrdemServico;
using _02_Application.Enums;
using _02_Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _05_Tests
{
    public class OrdemServicoTransicaoControllerTests
    {
        private sealed class FakeOrdemServicoTransicaoService : IOrdemServicoTransicaoService
        {
            public bool ConcluirChamado { get; private set; }
            public bool CancelarChamado { get; private set; }

            public OrdemServicoTransicaoResultadoDto ResultadoARetornar { get; set; } =
                new(OrdemServicoTransicaoResultado.Sucesso, "ok", 1);

            public Task<OrdemServicoTransicaoResultadoDto> ConcluirAsync(int ordemServicoId, int usuarioLogadoId)
            {
                ConcluirChamado = true;
                return Task.FromResult(ResultadoARetornar);
            }

            public Task<OrdemServicoTransicaoResultadoDto> CancelarAsync(int ordemServicoId, int usuarioLogadoId)
            {
                CancelarChamado = true;
                return Task.FromResult(ResultadoARetornar);
            }
        }

        private static OrdemServicoTransicaoController CriarController(
            FakeOrdemServicoTransicaoService servico,
            int? idUsuarioLogado = 1)
        {
            var controller = new OrdemServicoTransicaoController(servico);

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
        public async Task ConcluirAsync_DeveRetornar400_QuandoIdEhZeroOuNegativo()
        {
            var servico = new FakeOrdemServicoTransicaoService();
            var controller = CriarController(servico);

            IActionResult resultado = await controller.ConcluirAsync(0);

            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.False(servico.ConcluirChamado);
        }

        [Fact]
        public async Task ConcluirAsync_DeveRetornar401_QuandoNaoHaClaimDeUsuario()
        {
            var servico = new FakeOrdemServicoTransicaoService();
            var controller = CriarController(servico, idUsuarioLogado: null);

            IActionResult resultado = await controller.ConcluirAsync(1);

            Assert.IsType<UnauthorizedObjectResult>(resultado);
            Assert.False(servico.ConcluirChamado);
        }

        [Fact]
        public async Task ConcluirAsync_DeveRetornar200_QuandoServicoRetornaSucesso()
        {
            var servico = new FakeOrdemServicoTransicaoService
            {
                ResultadoARetornar = new OrdemServicoTransicaoResultadoDto(OrdemServicoTransicaoResultado.Sucesso, "ok", 42)
            };
            var controller = CriarController(servico);

            IActionResult resultado = await controller.ConcluirAsync(1);

            var ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, ok.StatusCode);
            Assert.True(servico.ConcluirChamado);
        }

        [Theory]
        [InlineData(OrdemServicoTransicaoResultado.NaoEncontrada, 404)]
        [InlineData(OrdemServicoTransicaoResultado.NaoAutorizado, 403)]
        [InlineData(OrdemServicoTransicaoResultado.EstadoInvalido, 409)]
        public async Task ConcluirAsync_DeveMapearCadaResultadoParaOCodigoHttpCorreto(
            OrdemServicoTransicaoResultado resultadoServico, int codigoHttpEsperado)
        {
            var servico = new FakeOrdemServicoTransicaoService
            {
                ResultadoARetornar = new OrdemServicoTransicaoResultadoDto(resultadoServico, "mensagem qualquer", null)
            };
            var controller = CriarController(servico);

            IActionResult resultado = await controller.ConcluirAsync(1);

            var objectResult = Assert.IsAssignableFrom<ObjectResult>(resultado);
            Assert.Equal(codigoHttpEsperado, objectResult.StatusCode);
        }

        [Fact]
        public async Task CancelarAsync_DeveRetornar400_QuandoIdEhZeroOuNegativo()
        {
            var servico = new FakeOrdemServicoTransicaoService();
            var controller = CriarController(servico);

            IActionResult resultado = await controller.CancelarAsync(-1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.False(servico.CancelarChamado);
        }

        [Fact]
        public async Task CancelarAsync_DeveRetornar401_QuandoNaoHaClaimDeUsuario()
        {
            var servico = new FakeOrdemServicoTransicaoService();
            var controller = CriarController(servico, idUsuarioLogado: null);

            IActionResult resultado = await controller.CancelarAsync(1);

            Assert.IsType<UnauthorizedObjectResult>(resultado);
            Assert.False(servico.CancelarChamado);
        }

        [Fact]
        public async Task CancelarAsync_DeveRetornar200_QuandoServicoRetornaSucesso()
        {
            var servico = new FakeOrdemServicoTransicaoService
            {
                ResultadoARetornar = new OrdemServicoTransicaoResultadoDto(OrdemServicoTransicaoResultado.Sucesso, "ok", 42)
            };
            var controller = CriarController(servico);

            IActionResult resultado = await controller.CancelarAsync(1);

            var ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, ok.StatusCode);
            Assert.True(servico.CancelarChamado);
        }

        [Theory]
        [InlineData(OrdemServicoTransicaoResultado.NaoEncontrada, 404)]
        [InlineData(OrdemServicoTransicaoResultado.NaoAutorizado, 403)]
        [InlineData(OrdemServicoTransicaoResultado.EstadoInvalido, 409)]
        public async Task CancelarAsync_DeveMapearCadaResultadoParaOCodigoHttpCorreto(
            OrdemServicoTransicaoResultado resultadoServico, int codigoHttpEsperado)
        {
            var servico = new FakeOrdemServicoTransicaoService
            {
                ResultadoARetornar = new OrdemServicoTransicaoResultadoDto(resultadoServico, "mensagem qualquer", null)
            };
            var controller = CriarController(servico);

            IActionResult resultado = await controller.CancelarAsync(1);

            var objectResult = Assert.IsAssignableFrom<ObjectResult>(resultado);
            Assert.Equal(codigoHttpEsperado, objectResult.StatusCode);
        }
    }
}
