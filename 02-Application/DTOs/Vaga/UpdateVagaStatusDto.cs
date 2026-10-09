using _04_Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.Vaga
{
    public class UpdateVagaStatusDto
    {
        [Required(ErrorMessage = "O status da vaga é obrigatório.")]
        [EnumDataType(typeof(StatusVaga), ErrorMessage = "O status da vaga informado é inválido.")]
        public StatusVaga? Status { get; set; }
    }
}
