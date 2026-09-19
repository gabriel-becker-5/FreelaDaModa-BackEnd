using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.User
{
    public class UpdateUserDto
    {
        [MaxLength(100)]
        public string? LegalResponsibleFullName { get; set; }

        [RegularExpression(@"^$|^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato do CPF incorreto.")]
        public string? LegalResponsibleDocument { get; set; } // CPF

        [RegularExpression(@"^$|[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Formato de e-mail inválido."), MaxLength(100)]
        public string? Email { get; set; }

        [RegularExpression(@"^$|^\d{10,11}$", ErrorMessage = "Informe um telefone com DDD (10 ou 11 dígitos).")]
        public string? ContactNumber { get; set; }

        [MaxLength(500, ErrorMessage = "A descrição aceita no máximo 500 caracteres.")]
        public string? PublicProfileDescription { get; set; }

        [RegularExpression(@"^$|^\d{5}-?\d{3}$", ErrorMessage = "O CEP deve conter 8 dígitos.")]
        public string? PostalCode { get; set; }

        [MaxLength(150)]
        public string? Address { get; set; }

        public int? AddressNumber { get; set; }

        [MaxLength(100)]
        public string? Neighborhood { get; set; }

        [MaxLength(150, ErrorMessage = "O complemento de endereço aceita no máximo 150 caracteres.")]
        public string? AdditionalAddressInfo { get; set; }

        [MaxLength(150)]
        public string? City { get; set; }

        [MaxLength(150)]
        public string? State { get; set; }
    }
}