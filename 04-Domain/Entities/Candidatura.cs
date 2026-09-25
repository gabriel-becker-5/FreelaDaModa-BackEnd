using _04_Domain.Enums;

namespace _04_Domain.Entities
{
    public class Candidatura
    {
        public int Id { get; set; }
        public int VagaId { get; set; }
        public Vaga? Vaga { get; set; }
        public int UsuarioId { get; set; }
        public int FreelancerId { get; set; }
        public DateTime DataCandidatura { get; set; }
        public StatusCandidatura Status { get; set; }
        public string? Mensagem { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}