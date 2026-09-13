namespace _04_Domain.Entities.ObjectsFields
{
    public class OwnMachine : BaseEntity
    {
        // Máquinas que possui: Reta, Overloque, Galoneira, Travete, Interlock, Máquina de corte.
        public string OwnMachineName { get; set; }
        public ICollection<FreelancerOwnMachines>? FreelancerOwnMachines { get; set; }
    }
}