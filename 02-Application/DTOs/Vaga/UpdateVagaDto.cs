using _04_Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.Vaga
{
    public class UpdateVagaDto
    {
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
        public string? Titulo { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string? Descricao { get; set; }

        [EnumDataType(typeof(Specialty), ErrorMessage = "A especialidade informada é inválida.")]
        public Specialty? Especialidade { get; set; }

        [EnumDataType(typeof(ModalidadeVaga), ErrorMessage = "A modalidade informada é inválida.")]
        public ModalidadeVaga? Modalidade { get; set; }

        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public decimal? Orcamento { get; set; }
        public DateTime? PrazoConclusao { get; set; }
    }
}
