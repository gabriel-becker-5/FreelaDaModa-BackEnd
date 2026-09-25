using _02_Application.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.Company
{
    public class UpdateCompanyDto : UpdateUserDto
    {
        // Campos de Empresa/Confecção
        [MaxLength(150)]
        public string? LegalName { get; set; } // Razão Social

        [MaxLength(150)]
        public string? CompanyName { get; set; } // Nome Fantasia

        [MaxLength(14, ErrorMessage = "O CNPJ deve ter exatamente 14 caracteres.")]
        [RegularExpression(@"\A[A-Z0-9]{8}[0-9]{6}\z", ErrorMessage = "O CNPJ informado é inválido. Não inclua caracteres especiais.")]
        public string? CompanyRegistrationDocument { get; set; } // CNPJ

        [MaxLength(150)]
        public string? CoreBusiness { get; set; } // Ramo de Atuação
    }
}