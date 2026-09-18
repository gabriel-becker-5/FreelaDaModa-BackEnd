using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.Company
{
    public class CreateCompanyDto
    {
        [Required(ErrorMessage = "O Nome do responsável legal é obrigatório."), MaxLength(100)]
        public string LegalResponsibleFullName { get; set; }

        [Required(ErrorMessage = "O CPF do responsável legal é obrigatório.")]
        [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato do CPF incorreto.")]
        public string LegalResponsibleDocument { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório."), MaxLength(100)]
        [EmailAddress(ErrorMessage = "Informe um E-mail válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória."), MaxLength(50)]
        [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z\s]).{10,}$",
    ErrorMessage = "A senha deve ter no mínimo 10 caracteres, incluindo maiúscula, minúscula, número e caractere especial.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O telefone/celular é obrigatório.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Informe um telefone com DDD (10 ou 11 dígitos).")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "O CEP deve conter 8 dígitos.")]
        public string PostalCode { get; set; }
        
        [Required(ErrorMessage = "O Endereço é obrigatório."), MaxLength(150)]
        public string Address { get; set; }

        [Required(ErrorMessage = "O número do endereço é obrigatório.")]
        public int AddressNumber { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório."), MaxLength(100)]
        public string Neighborhood { get; set; }

        [MaxLength(150, ErrorMessage = "O complemento de endereço aceita no máximo 150 caracteres.")]
        public string? AdditionalAddressInfo { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória."), MaxLength(150)]
        public string City { get; set; }

        [Required(ErrorMessage = "O estado é obrigatório."), MaxLength(150)]
        public string State { get; set; }

        [Required(ErrorMessage = "A descrição pública do perfil é obrigatória."), 
         MaxLength(500, ErrorMessage = "A descrição aceita no máximo 500 caracteres.")]
        public string PublicProfileDescription { get; set; }

        // Campos de Empresa/Confecção
        [Required(ErrorMessage = "A razão social é obrigatória."), MaxLength(150)]
        public string LegalName { get; set; }

        [Required(ErrorMessage = "O nome fantasia é obrigatório."), MaxLength(150)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [RegularExpression(@"^[A-Z0-9]{12}\d{2}$", ErrorMessage = "O formato do CNPJ informado é inválido.")]
        public string CompanyRegistrationDocument { get; set; }

        [Required(ErrorMessage = "O ramo de atuação é obrigatório."), MaxLength(150)]
        public string CoreBusiness { get; set; }
    }
}