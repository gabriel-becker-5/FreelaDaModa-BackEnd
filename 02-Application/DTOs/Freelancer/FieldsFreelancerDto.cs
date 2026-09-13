using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.Freelancer
{
    public class FieldsFreelancerDto
    {
        [Required(ErrorMessage = "O nome do campo é obrigatório.")]
        public string RegisterName { get; set; }
    }
}