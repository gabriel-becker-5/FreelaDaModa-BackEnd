using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_Domain.Entities
{
    public class Mensagem
    {
        public int Id { get; set; }

        public int RemetenteId { get; set; }

        public int DestinatarioId { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataEnvio { get; set; }
    }
}