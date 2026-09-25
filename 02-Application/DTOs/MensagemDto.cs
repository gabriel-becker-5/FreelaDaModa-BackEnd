using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_Application.DTOs
{
    public class MensagemDto
    {
        public int RemetenteId { get; set; }

        public int DestinatarioId { get; set; }

        public string Conteudo { get; set; }
    }
}
