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

        public int UserId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int Nota { get; set; }

        public string Comentario { get; set; } = string.Empty;
    }
}

