namespace _04_Domain.Entities
{
    public class Candidatura
    {
        public int Id { get; set; }
        public int VagaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataCandidatura { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Em Análise";
        public Vaga? Vaga { get; set; }
    }
}