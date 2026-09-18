using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_Domain.Entities
{
    public class Vaga // <-- Garanta que tem a palavra public aqui
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Orcamento { get; set; }
        public DateTime DataPublicacao { get; set; } = DateTime.Now;
        public bool Ativa { get; set; } = true;
        public int UsuarioId { get; set; }
    }
}