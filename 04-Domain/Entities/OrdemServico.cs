using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace _04_Domain.Entities
{
    public class OrdemServico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [MaxLength(200)]
        public string Titulo { get; set; }

        [MaxLength(1000)]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "A modalidade é obrigatória.")]
        public string Modalidade { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string Cidade { get; set; }

        public decimal Valor { get; set; }

        public DateTime Prazo { get; set; }

        public string? Status { get; set; }

        public string? Observacoes { get; set; }

        public int? FreelancerId { get; set; }
    }
}
