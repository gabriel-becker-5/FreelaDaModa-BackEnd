namespace _02_Application.DTOs.Vaga
{
    public class RespostaVagaJson
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Orcamento { get; set; }
        public DateTime DataPublicacao { get; set; }
        public bool Ativa { get; set; }
        public int UsuarioId { get; set; }
    }
}