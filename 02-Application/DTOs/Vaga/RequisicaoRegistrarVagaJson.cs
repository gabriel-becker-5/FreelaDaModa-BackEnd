namespace _02_Application.DTOs.Vaga
{
    public class RequisicaoRegistrarVagaJson
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Orcamento { get; set; }
        public int UsuarioId { get; set; }
    }
}