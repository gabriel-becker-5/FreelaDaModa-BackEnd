using _04_Domain.Entities.Profiles;

namespace _04_Domain.Entities.ObjectsFields
{
    public class FreelancerOwnMachines : BaseEntity
    {
        public int FreelancerId { get; set; }
        public FreelancerProfile Freelancer { get; set; }
        public int OwnMachineId { get; set; }
        public OwnMachine OwnMachine { get; set; }
    }
}