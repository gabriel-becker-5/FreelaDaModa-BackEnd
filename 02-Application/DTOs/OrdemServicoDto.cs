using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_Application.DTOs
{
    public class OrdemServicoDto
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string? Descricao { get; set; }

        public string Categoria { get; set; }

        public string Modalidade { get; set; }

        public string Cidade { get; set; }

        public decimal Valor { get; set; }

        public DateTime Prazo { get; set; }

        public string? Status { get; set; }

        public string? Observacoes { get; set; }

        public int? FreelancerId { get; set; }
    }
}
