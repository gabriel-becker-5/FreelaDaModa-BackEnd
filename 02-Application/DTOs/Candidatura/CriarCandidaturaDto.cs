using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.Candidatura
{
    public class CriarCandidaturaDto
    {
        [Required(ErrorMessage = "A identificação da vaga é obrigatória.")]
        public int VagaId { get; set; }

        [MaxLength(500, ErrorMessage = "A mensagem opcional aceita no máximo 500 caracteres.")]
        public string? Mensagem { get; set; }
    }
}