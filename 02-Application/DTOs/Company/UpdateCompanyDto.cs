using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.Company
{
    public class UpdateCompanyDto
    {
        public string? LegalResponsibleFullName { get; set; }
        [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato do CPF incorreto.")]
        public string? LegalResponsibleDocument { get; set; } // CPF

        [EmailAddress(ErrorMessage = "Informe um E-mail válido.")]
        public string? Email { get; set; }
        
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Informe um telefone com DDD (10 ou 11 dígitos).")]
        public string? ContactNumber { get; set; }
        
        [RegularExpression(@"^\d{8}$", ErrorMessage = "O CEP deve conter 8 dígitos.")]
        public string? PostalCode { get; set; }
        public string? Address { get; set; }
        public int? AddressNumber { get; set; }
        public string? Quarter { get; set; }

        [MaxLength(150, ErrorMessage = "O complemento de endereço aceita no máximo 150 caracteres.")]
        public string? AdditionalAddressInfo { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }

        [MaxLength(300, ErrorMessage = "A descrição aceita no máximo 300 caracteres.")]
        public string? PublicProfileDescription { get; set; }

        // Campos de Empresa/Confecção
        public string? LegalName { get; set; } // Razão Social
        public string? CompanyName { get; set; } // Nome Fantasia
        
        [RegularExpression(@"^[A-Z0-9]{12}\d{2}$", ErrorMessage = "O formato do CNPJ informado é inválido.")]
        public string? CompanyRegistrationDocument { get; set; } // CNPJ
        public string? CoreBusiness { get; set; } // Ramo de Atuação
    }
}