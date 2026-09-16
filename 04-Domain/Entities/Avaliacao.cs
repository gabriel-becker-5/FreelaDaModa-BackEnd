using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_Domain.Entities
{
    public class Avaliacao
    {
        
        public int Id { get; set; }

        public int OrdemServicoId { get; set; }

        [Range(1, 5, ErrorMessage = "A nota deve estar entre 1 e 5.")]
        public int Nota { get; set; }

        [Required(ErrorMessage = "O comentário é obrigatório.")]
        public string Comentario { get; set; }
    }
}

