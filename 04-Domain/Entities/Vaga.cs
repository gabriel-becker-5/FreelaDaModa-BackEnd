using _04_Domain.Enums;

namespace _04_Domain.Entities
{
    public class Vaga : BaseEntity
    {
        public string Titulo { get; set; } = string.Empty;
        public Specialty Especialidade { get; set; }
        public ModalidadeVaga Modalidade { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal Orcamento { get; set; }
        public DateTime PrazoConclusao { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public StatusVaga Status { get; set; } = StatusVaga.Aberta;
        public bool Ativa { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int UsuarioId { get; set; }
    }
}