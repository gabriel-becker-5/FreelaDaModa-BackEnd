using System.ComponentModel.DataAnnotations;

public class RequisicaoRegistrarVagaJson
{
    [Required(ErrorMessage = "O título da vaga é obrigatório.")]
    [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição da vaga é obrigatória.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
    public string Empresa { get; set; } = string.Empty;

    [Required(ErrorMessage = "O local da vaga é obrigatório.")]
    public string Local { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "O salário deve ser um valor maior que zero.")]
    public decimal Salario { get; set; }
}