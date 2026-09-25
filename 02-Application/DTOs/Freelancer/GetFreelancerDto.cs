using _02_Application.DTOs.User;

namespace _02_Application.DTOs.Freelancer
{
    public class GetFreelancerDto : UserDto
    {
        // Campos de Freelancer
        public DateTime BirthDate { get; set; }
        public string AvailableTimeName { get; set; }
        public string ExperienceYearsName { get; set; }
        public ICollection<string> SpecialtyNames { get; set; }
        public ICollection<string> OwnMachineNames { get; set; }
    }
}