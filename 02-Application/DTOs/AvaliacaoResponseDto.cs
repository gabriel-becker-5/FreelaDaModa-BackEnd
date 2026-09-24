using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
