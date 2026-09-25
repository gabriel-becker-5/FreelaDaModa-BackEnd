namespace _02_Application.DTOs
{
    public class AvaliacaoResponseDto
    {
        public int Id { get; set; }
        public int OrdemServicoId { get; set; }
        public int UserId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}