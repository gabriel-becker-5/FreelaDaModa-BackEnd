namespace _02_Application.DTOs.Freelancer
{
    public class FreelancerPublicProfileDto
    {
        public string LegalResponsibleFullName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public bool IsVerified { get; set; }
        public string PublicProfileDescription { get; set; }
        public ICollection<string> SpecialtyNames { get; set; }
        public ICollection<string> OwnMachineNames { get; set; }
        public string ExperienceYearsName { get; set; }
        public string AvailableTimeName { get; set; }
    }
}