namespace _02_Application.DTOs.Freelancer
{
    public class FreelancerProfileDto
    {
        public int Id { get; set; }
        public string BusinessTypeName { get; set; }
        public string ExperienceYearsName { get; set; }
        public string WorkshopSizeName { get; set; }
        public string HowUsuallyArrangeServicesName { get; set; }
        public string AvailableTimeName { get; set; }
        public string FreelancerPreferencesName { get; set; }
        public string AverageRevenueName { get; set; }
        public bool HasFixedProducer { get; set; }
        public bool HasOwnCar { get; set; }
        public ICollection<string> Specialties { get; set; }
        public ICollection<string> OwnMachines { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
