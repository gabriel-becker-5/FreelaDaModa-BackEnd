using _04_Domain.Enums;

namespace _05_Tests
{
    public class StatusOrdemServicoTests
    {
        [Fact]
        public void TryParse_DeveRetornarFalso_QuandoTextoForNulo()
        {
            bool resultado = StatusOrdemServicoExtensions.TryParse(null, out StatusOrdemServico status);

            Assert.False(resultado);
            Assert.Equal(StatusOrdemServico.EmAndamento, status);
        }

        [Theory]
        [InlineData("concluida")]
        [InlineData("Concluída")]
        [InlineData("CONCLUIDA")]
        [InlineData("concluido")]
        [InlineData("finalizada")]
        [InlineData("finalizado")]
        public void TryParse_DeveReconhecerValoresLegadosDeConcluida(string texto)
        {
            bool resultado = StatusOrdemServicoExtensions.TryParse(texto, out StatusOrdemServico status);

            Assert.True(resultado);
            Assert.Equal(StatusOrdemServico.Concluida, status);
        }

        [Theory]
        [InlineData("Em andamento")]
        [InlineData("em andamento")]
        [InlineData("EM ANDAMENTO")]
        public void TryParse_DeveReconhecerEmAndamento(string texto)
        {
            bool resultado = StatusOrdemServicoExtensions.TryParse(texto, out StatusOrdemServico status);

            Assert.True(resultado);
            Assert.Equal(StatusOrdemServico.EmAndamento, status);
        }

        [Theory]
        [InlineData("Cancelada")]
        [InlineData("cancelado")]
        public void TryParse_DeveReconhecerCancelada(string texto)
        {
            bool resultado = StatusOrdemServicoExtensions.TryParse(texto, out StatusOrdemServico status);

            Assert.True(resultado);
            Assert.Equal(StatusOrdemServico.Cancelada, status);
        }

        [Fact]
        public void TryParse_DeveRetornarFalso_QuandoTextoForDesconhecido()
        {
            bool resultado = StatusOrdemServicoExtensions.TryParse("xpto", out _);

            Assert.False(resultado);
        }

        [Theory]
        [InlineData(StatusOrdemServico.EmAndamento, "Em andamento")]
        [InlineData(StatusOrdemServico.Concluida, "Concluída")]
        [InlineData(StatusOrdemServico.Cancelada, "Cancelada")]
        public void ParaTexto_DeveRetornarOTextoExatoDoContratoComOFront(StatusOrdemServico status, string textoEsperado)
        {
            Assert.Equal(textoEsperado, status.ParaTexto());
        }
    }
}
