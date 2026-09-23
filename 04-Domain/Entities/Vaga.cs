using System;
using _04_Domain.Enums; // <-- Adiciona esta linha no topo

namespace _04_Domain.Entities
{
    public class Vaga
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Orcamento { get; set; }
        public DateTime DataPublicacao { get; set; } = DateTime.Now;
        public bool Ativa { get; set; } = true;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public StatusVaga Status { get; set; } = StatusVaga.Aberta;
        public int UsuarioId { get; set; }
    }
}