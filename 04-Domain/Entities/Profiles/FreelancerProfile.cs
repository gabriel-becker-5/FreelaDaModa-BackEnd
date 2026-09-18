using _04_Domain.Entities.Identity;
using _04_Domain.Enums;

namespace _04_Domain.Entities.Profiles
{
    public class FreelancerProfile : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime BirthDate { get; set; }
        public BusinessType BusinessTypeId { get; set; } // Tipo de Negócio: MEI, ME, SEM CNPJ
        public ExperienceYears ExperienceYearsId { get; set; } // Tempo de Experiência: Menos de 1 ano, 1 a 3 anos, 3 a 5 anos, 5 a 10 anos, mais de 10 anos.
        public WorkshopSize WorkshopSizeId { get; set; } // Tamanho da Oficina: Apenas eu, 2 a 3 pessoas, 4 a 6 pessoas, mais de 6 pessoas.
        public ICollection<Specialty> SpecialtiesIds { get; set; } // Especialidades: Costura reta, Overloque, Modelagem, Peça piloto, Amostras, Acabamento, Bordado, Estamparia.
        public ICollection<OwnMachine> OwnMachinesIds { get; set; } // Máquinas que possui: Reta, Overloque, Galoneira, Travete, Interlock, Máquina de corte.
        public HowUsuallyArrangeServices HowUsuallyArrangeServicesId { get; set; } // Como costuma fechar serviços: Grupos whatsapp, indicação, redes sociais, plataformas online, outros (cite).
        public AvailableTime AvailableTimeId { get; set; } // Disponibilidade de Tempo: Período integral, meio período, fins de semana, sob demanda.
        public FreelancerPreferences FreelancerPreferencesId { get; set; } // Preferências do Freelancer: Serviços pontuais, contrato fixo, grandes lotes, peças exclusivas/autorais.
        public AverageRevenue AverageRevenueId { get; set; } // Faturamento médio atual: até R$ 1000, de R$ 1000 a R$ 3000 mil, de R$ 3000 A R$ 6000, acima de R$ 6000, prefiro não informar.
        public bool HasFixedProducer { get; set; } // Tem produtor fixo? Sim, Não
        public bool HasOwnCar { get; set; } // Tem veículo para buscar/levar peças? Sim, Não
    }
}