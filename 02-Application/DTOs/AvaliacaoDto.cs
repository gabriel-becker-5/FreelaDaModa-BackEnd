using System.ComponentModel.DataAnnotations;


namespace _02_Application.DTOs
{
    public class AvaliacaoDto
    {
        public int OrdemServicoId { get; set; }

        [Range(1, 5, ErrorMessage = "A nota deve estar entre 1 e 5.")]
        public int Nota { get; set; }

        [Required(ErrorMessage = "O comentário é obrigatório.")]
        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;
    }
}