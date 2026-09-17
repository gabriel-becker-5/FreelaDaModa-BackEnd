using _02_Application.DTOs.User;

namespace _02_Application.DTOs.Freelancer
{
    public class UpdateFreelancerDto : UpdateUserDto
    {
        // Campos de Freelancer
        public DateTime? BirthDate { get; set; }
        public int? BusinessTypeId { get; set; }
        public int? ExperienceYearsId { get; set; }
        public int? WorkshopSizeId { get; set; }
        public IEnumerable<int> SpecialtyIds { get; set; }
        public IEnumerable<int> OwnMachineIds { get; set; }
        public int? HowUsuallyArrangeServicesId { get; set; }
        public int? AvailableTimeId { get; set; }
        public int? FreelancerPreferencesId { get; set; }
        public int? AverageRevenueId { get; set; }
        public bool? HasFixedProducer { get; set; }
        public bool? HasOwnCar { get; set; }
    }
}