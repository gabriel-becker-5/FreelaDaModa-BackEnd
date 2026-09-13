using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.Freelancer
{
    public class UpdateFreelancerDto
    {
        public string? LegalResponsibleFullName { get; set; }

        [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato do CPF incorreto.")]
        public string? LegalResponsibleDocument { get; set; }


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


        // Campos de Freelancer
        public DateTime? BirthDate { get; set; }
        public int? BusinessTypeId { get; set; }
        public int? ExperienceYearsId { get; set; }
        public int? WorkshopSizeId { get; set; }
        public ICollection<int> SpecialtyIds { get; set; }
        public ICollection<int> OwnMachineIds { get; set; }
        public int? HowUsuallyArrangeServicesId { get; set; }
        public int? AvailableTimeId { get; set; }
        public int? FreelancerPreferencesId { get; set; }
        public int? AverageRevenueId { get; set; }
        public bool? HasFixedProducer { get; set; }
        public bool? HasOwnCar { get; set; }
    }
}