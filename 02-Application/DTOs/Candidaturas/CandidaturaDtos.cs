using _04_Domain.Enums;

namespace _02_Application.DTOs.Candidaturas
{
    // Resposta de criação e de atualização de status
    public class CandidaturaDto
    {
        public int Id { get; set; }
        public int VagaId { get; set; }
        public int FreelancerId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataCandidatura { get; set; }
        public string? Mensagem { get; set; }
        public StatusCandidatura Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Resposta de GET candidaturas/minhas
    public class MinhaCandidaturaDto
    {
        public int Id { get; set; }
        public int VagaId { get; set; }
        public string TituloVaga { get; set; } = string.Empty;
        public DateTime DataCandidatura { get; set; }
        public DateTime CreatedAt { get; set; }
        public StatusCandidatura Status { get; set; }
        public string? Mensagem { get; set; }
    }

    // Resposta de GET candidaturas (com filtros)
    public class CandidaturaResumoDto
    {
        public int Id { get; set; }
        public int VagaId { get; set; }
        public string TituloVaga { get; set; } = string.Empty;
        public int FreelancerId { get; set; }
        public DateTime DataCandidatura { get; set; }
        public DateTime CreatedAt { get; set; }
        public StatusCandidatura Status { get; set; }
        public string? Mensagem { get; set; }
    }
}