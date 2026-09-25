using _04_Domain.Entities.Identity;
using _04_Domain.Enums;

namespace _04_Domain.Entities.Profiles
{
    public class FreelancerProfile : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ExperienceYears ExperienceYearsId { get; set; } // Tempo de Experiência: Menos de 1 ano, 1 a 3 anos, 3 a 5 anos, 5 a 10 anos, mais de 10 anos.
        public AvailableTime AvailableTimeId { get; set; } // Disponibilidade de Tempo: Período integral, meio período, fins de semana, sob demanda.
        public ICollection<Specialty> SpecialtiesIds { get; set; } // Especialidades: Costura reta, Overloque, Modelagem, Peça piloto, Amostras, Acabamento, Bordado, Estamparia.
        public ICollection<OwnMachine> OwnMachinesIds { get; set; } // Máquinas que possui: Reta, Overloque, Galoneira, Travete, Interlock, Máquina de corte.
    }
}