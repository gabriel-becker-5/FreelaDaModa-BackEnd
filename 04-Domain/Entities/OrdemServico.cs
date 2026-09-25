using System;

namespace _04_Domain.Entities
{
    public class OrdemServico
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string Titulo { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public string Categoria { get; set; } = string.Empty;

        public string Modalidade { get; set; } = string.Empty;

        public string Cidade { get; set; } = string.Empty;

        public decimal Valor { get; set; }

        public DateTime Prazo { get; set; }

        public string? Status { get; set; }

        public string? Observacoes { get; set; }

        public int? FreelancerId { get; set; }
    }
}
