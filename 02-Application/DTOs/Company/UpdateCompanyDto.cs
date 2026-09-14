using System.ComponentModel.DataAnnotations;
using _02_Application.DTOs.User;

namespace _02_Application.DTOs.Company
{
    public class UpdateCompanyDto : UserDto
    {
        // Campos de Empresa/Confecção
        public string? LegalName { get; set; } // Razão Social
        public string? CompanyName { get; set; } // Nome Fantasia

        [RegularExpression(@"^[A-Z0-9]{12}\d{2}$", ErrorMessage = "O formato do CNPJ informado é inválido.")]
        public string? CompanyRegistrationDocument { get; set; } // CNPJ
        public string? CoreBusiness { get; set; } // Ramo de Atuação
    }
}
