using System.ComponentModel.DataAnnotations;
using _02_Application.DTOs.User;

namespace _02_Application.DTOs.Company
{
    public class UpdateCompanyDto : UpdateUserDto
    {
        // Campos de Empresa/Confecção
        [MaxLength(150)]
        public string? LegalName { get; set; } // Razão Social

        [MaxLength(150)]
        public string? CompanyName { get; set; } // Nome Fantasia

        [RegularExpression(@"^$|^[A-Z0-9]{12}\d{2}$", ErrorMessage = "O CNPJ informado é inválido.")]
        public string? CompanyRegistrationDocument { get; set; } // CNPJ

        [MaxLength(150)]
        public string? CoreBusiness { get; set; } // Ramo de Atuação
    }
}