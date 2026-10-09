using _04_Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class RequisicaoRegistrarVagaJson
{
    [Required(ErrorMessage = "O título da vaga é obrigatório.")]
    [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A especialidade da vaga é obrigatória.")]
    [EnumDataType(typeof(Specialty), ErrorMessage = "A especialidade informada é inválida.")]
    public Specialty? Especialidade { get; set; }

    [Required(ErrorMessage = "A modalidade da vaga é obrigatória.")]
    [EnumDataType(typeof(ModalidadeVaga), ErrorMessage = "A modalidade informada é inválida.")]
    public ModalidadeVaga? Modalidade { get; set; }

    [Required(ErrorMessage = "A cidade da vaga é obrigatória.")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "O estado da vaga é obrigatório.")]
    public string Estado { get; set; } = string.Empty;

    public decimal Orcamento { get; set; }

    [Required(ErrorMessage = "O prazo de conclusão é obrigatório.")]
    public DateTime? PrazoConclusao { get; set; }

    [Required(ErrorMessage = "A descrição da vaga é obrigatória.")]
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string Descricao { get; set; } = string.Empty;
}
