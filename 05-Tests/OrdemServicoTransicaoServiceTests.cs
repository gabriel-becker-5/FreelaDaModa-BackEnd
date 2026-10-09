using _02_Application.DTOs.OrdemServico;
using _02_Application.Enums;
using _03_Infrastructure.Data;
using _03_Infrastructure.Services;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace _05_Tests
{
    public class OrdemServicoTransicaoServiceTests : IDisposable
    {
        private const int IdEmpresaDona = 1;
        private const int IdOutraEmpresa = 2;
        private const int IdFreelancerDaOs = 10;
        private const int IdOutroFreelancer = 11;
        private const int IdOrdemServico = 100;

        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _options;

        public OrdemServicoTransicaoServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using AppDbContext context = new(_options);
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        private AppDbContext NovoContexto() => new(_options);

        private static OrdemServicoTransicaoService CriarServico(AppDbContext context) => new(context);

        private static User CriarUsuario(int id, Roles role, bool isDeleted = false)
        {
            return new User
            {
                Id = id,
                Email = $"usuario{id}@teste.com",
                LegalResponsibleFullName = "Responsável Teste",
                LegalResponsibleDocument = $"{id:D11}",
                PasswordHash = "hash",
                ContactNumber = "11999999999",
                PublicProfileDescription = "descrição",
                PostalCode = "01000000",
                Address = "Rua Teste",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Roles = new List<Roles> { role },
                IsDeleted = isDeleted
            };
        }

        private async Task SeedAsync(
            AppDbContext context,
            string status = "Em andamento",
            bool isDeleted = false,
            int? freelancerId = IdFreelancerDaOs,
            bool empresaDonaExcluida = false,
            bool freelancerDaOsExcluido = false)
        {
            context.Users.Add(CriarUsuario(IdEmpresaDona, Roles.Company, isDeleted: empresaDonaExcluida));
            context.Users.Add(CriarUsuario(IdOutraEmpresa, Roles.Company));
            context.Users.Add(CriarUsuario(IdFreelancerDaOs, Roles.Freelancer, isDeleted: freelancerDaOsExcluido));
            context.Users.Add(CriarUsuario(IdOutroFreelancer, Roles.Freelancer));

            context.OrdensServico.Add(new OrdemServico
            {
                Id = IdOrdemServico,
                UserId = IdEmpresaDona,
                FreelancerId = freelancerId,
                Titulo = "Confecção de camisetas",
                Categoria = "Costura",
                Modalidade = "Remoto",
                Cidade = "São Paulo",
                Valor = 500m,
                Prazo = DateTime.UtcNow.AddDays(5),
                Status = status,
                IsDeleted = isDeleted
            });

            await context.SaveChangesAsync();
        }

        // ---------- Concluir ----------

        [Fact]
        public async Task ConcluirAsync_DeveTerSucesso_QuandoEmpresaDonaConcluiOsEmAndamento()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, resultado.Resultado);
            Assert.Equal(IdOrdemServico, resultado.OrdemServicoId);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Concluída", ordem.Status);
        }

        [Fact]
        public async Task ConcluirAsync_DeveRetornarNaoAutorizado_QuandoChamadoPeloFreelancerDaOs()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdFreelancerDaOs);

            Assert.Equal(OrdemServicoTransicaoResultado.NaoAutorizado, resultado.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Em andamento", ordem.Status);
        }

        [Fact]
        public async Task ConcluirAsync_DeveRetornarNaoAutorizado_QuandoChamadoPorOutraEmpresa()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdOutraEmpresa);

            Assert.Equal(OrdemServicoTransicaoResultado.NaoAutorizado, resultado.Resultado);
        }

        [Fact]
        public async Task ConcluirAsync_DeveRetornarNaoEncontrada_QuandoOsNaoExiste()
        {
            await using AppDbContext setup = NovoContexto();
            setup.Users.Add(CriarUsuario(IdEmpresaDona, Roles.Company));
            await setup.SaveChangesAsync();

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(9999, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.NaoEncontrada, resultado.Resultado);
        }

        [Fact]
        public async Task ConcluirAsync_DeveRetornarNaoEncontrada_QuandoOsEstaExcluida()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, isDeleted: true);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.NaoEncontrada, resultado.Resultado);
        }

        [Fact]
        public async Task ConcluirAsync_DeveRetornarEstadoInvalido_QuandoOsJaEstaConcluida()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, status: "Concluída");

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.EstadoInvalido, resultado.Resultado);
        }

        [Fact]
        public async Task ConcluirAsync_DeveRetornarEstadoInvalido_QuandoOsJaEstaCancelada()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, status: "Cancelada");

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.EstadoInvalido, resultado.Resultado);
        }

        [Fact]
        public async Task ConcluirAsync_DeveTerSucesso_QuandoStatusLegadoDivergeDoTextoCanonico()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, status: "EM ANDAMENTO ");

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, resultado.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Concluída", ordem.Status);
        }

        [Fact]
        public async Task ConcluirAsync_NaoDeveSerBloqueado_QuandoPrazoJaVenceu()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contextoPrazo = NovoContexto();
            OrdemServico ordemParaVencer = await contextoPrazo.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            ordemParaVencer.Prazo = DateTime.UtcNow.AddDays(-30);
            await contextoPrazo.SaveChangesAsync();

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, resultado.Resultado);
        }

        // ---------- Cancelar ----------

        [Fact]
        public async Task CancelarAsync_DeveTerSucesso_QuandoEmpresaDonaCancela()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, resultado.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Cancelada", ordem.Status);
        }

        [Fact]
        public async Task CancelarAsync_DeveTerSucesso_QuandoFreelancerDaOsCancela()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdFreelancerDaOs);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, resultado.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Cancelada", ordem.Status);
        }

        [Fact]
        public async Task CancelarAsync_DeveRetornarNaoAutorizado_QuandoFreelancerNaoEhODaOs()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdOutroFreelancer);

            Assert.Equal(OrdemServicoTransicaoResultado.NaoAutorizado, resultado.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Em andamento", ordem.Status);
        }

        [Fact]
        public async Task CancelarAsync_DeveRetornarNaoAutorizado_QuandoChamadoPorOutraEmpresa()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdOutraEmpresa);

            Assert.Equal(OrdemServicoTransicaoResultado.NaoAutorizado, resultado.Resultado);
        }

        [Fact]
        public async Task CancelarAsync_DeveRetornarEstadoInvalido_QuandoOsJaEstaCancelada()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, status: "Cancelada");

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.EstadoInvalido, resultado.Resultado);
        }

        [Fact]
        public async Task CancelarAsync_DeveRetornarEstadoInvalido_QuandoOsJaEstaConcluida()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, status: "Concluída");

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdFreelancerDaOs);

            Assert.Equal(OrdemServicoTransicaoResultado.EstadoInvalido, resultado.Resultado);
        }

        [Fact]
        public async Task CancelarAsync_DeveTerSucesso_QuandoStatusLegadoDivergeDoTextoCanonico()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, status: "EM ANDAMENTO ");

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdFreelancerDaOs);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, resultado.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Cancelada", ordem.Status);
        }

        [Fact]
        public async Task CancelarAsync_NaoDeveSerBloqueado_QuandoPrazoJaVenceu()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contextoPrazo = NovoContexto();
            OrdemServico ordemParaVencer = await contextoPrazo.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            ordemParaVencer.Prazo = DateTime.UtcNow.AddDays(-30);
            await contextoPrazo.SaveChangesAsync();

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdFreelancerDaOs);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, resultado.Resultado);
        }

        // ---------- Não-participante não descobre o estado ----------

        [Fact]
        public async Task ConcluirAsync_NaoParticipante_DeveRetornarNaoAutorizado_MesmoComOsJaFinalizada()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, status: "Concluída");

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdOutraEmpresa);

            Assert.Equal(OrdemServicoTransicaoResultado.NaoAutorizado, resultado.Resultado);
        }

        [Fact]
        public async Task CancelarAsync_NaoParticipante_DeveRetornarNaoAutorizado_MesmoComOsJaFinalizada()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, status: "Cancelada");

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdOutroFreelancer);

            Assert.Equal(OrdemServicoTransicaoResultado.NaoAutorizado, resultado.Resultado);
        }

        // ---------- Usuário inativo ----------

        [Fact]
        public async Task ConcluirAsync_DeveRetornarUsuarioInativo_QuandoEmpresaDonaEstaExcluida()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, empresaDonaExcluida: true);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.UsuarioInativo, resultado.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Em andamento", ordem.Status);
        }

        [Fact]
        public async Task CancelarAsync_DeveRetornarUsuarioInativo_QuandoFreelancerDaOsEstaExcluido()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup, freelancerDaOsExcluido: true);

            await using AppDbContext contexto = NovoContexto();
            OrdemServicoTransicaoService servico = CriarServico(contexto);

            OrdemServicoTransicaoResultadoDto resultado = await servico.CancelarAsync(IdOrdemServico, IdFreelancerDaOs);

            Assert.Equal(OrdemServicoTransicaoResultado.UsuarioInativo, resultado.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Em andamento", ordem.Status);
        }

        // ---------- Concorrência ----------

        [Fact]
        public async Task ConcluirECancelar_EmSequenciaNaMesmaOs_SoOPrimeiroDeveVencer()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto1 = NovoContexto();
            OrdemServicoTransicaoService servico1 = CriarServico(contexto1);
            OrdemServicoTransicaoResultadoDto primeira = await servico1.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            await using AppDbContext contexto2 = NovoContexto();
            OrdemServicoTransicaoService servico2 = CriarServico(contexto2);
            OrdemServicoTransicaoResultadoDto segunda = await servico2.CancelarAsync(IdOrdemServico, IdFreelancerDaOs);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, primeira.Resultado);
            Assert.Equal(OrdemServicoTransicaoResultado.EstadoInvalido, segunda.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Concluída", ordem.Status);
        }

        [Fact]
        public async Task CancelarEConcluir_EmSequenciaNaMesmaOs_SoOPrimeiroDeveVencer()
        {
            await using AppDbContext setup = NovoContexto();
            await SeedAsync(setup);

            await using AppDbContext contexto1 = NovoContexto();
            OrdemServicoTransicaoService servico1 = CriarServico(contexto1);
            OrdemServicoTransicaoResultadoDto primeira = await servico1.CancelarAsync(IdOrdemServico, IdFreelancerDaOs);

            await using AppDbContext contexto2 = NovoContexto();
            OrdemServicoTransicaoService servico2 = CriarServico(contexto2);
            OrdemServicoTransicaoResultadoDto segunda = await servico2.ConcluirAsync(IdOrdemServico, IdEmpresaDona);

            Assert.Equal(OrdemServicoTransicaoResultado.Sucesso, primeira.Resultado);
            Assert.Equal(OrdemServicoTransicaoResultado.EstadoInvalido, segunda.Resultado);

            await using AppDbContext verificacao = NovoContexto();
            OrdemServico ordem = await verificacao.OrdensServico.SingleAsync(o => o.Id == IdOrdemServico);
            Assert.Equal("Cancelada", ordem.Status);
        }
    }
}
