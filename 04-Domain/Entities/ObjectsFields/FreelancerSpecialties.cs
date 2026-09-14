using _04_Domain.Entities.Profiles;

namespace _04_Domain.Entities.ObjectsFields
{
    public class FreelancerSpecialties : BaseEntity
    {
        public int FreelancerId { get; set; }
        public FreelancerProfile Freelancer { get; set; }
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
    }
}