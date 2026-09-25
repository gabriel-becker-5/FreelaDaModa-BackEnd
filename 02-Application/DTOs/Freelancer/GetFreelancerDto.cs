using _02_Application.DTOs.User;

namespace _02_Application.DTOs.Freelancer
{
    public class GetFreelancerDto : UserDto
    {
        // Campos de Freelancer
        public DateTime BirthDate { get; set; }
        public string BusinessTypeName { get; set; }
        public string ExperienceYearsName { get; set; }
        public string WorkshopSizeName { get; set; }
        public ICollection<string> SpecialtyNames { get; set; }
        public ICollection<string> OwnMachineNames { get; set; }
        public string HowUsuallyArrangeServicesName { get; set; }
        public string AvailableTimeName { get; set; }
        public string FreelancerPreferencesName { get; set; }
        public string AverageRevenueName { get; set; }
        public bool HasFixedProducer { get; set; }
        public bool HasOwnCar { get; set; }
    }
}
