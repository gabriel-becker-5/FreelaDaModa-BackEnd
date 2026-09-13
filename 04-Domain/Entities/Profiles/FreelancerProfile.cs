using _04_Domain.Entities.ObjectsFields;
using _04_Domain.Entities.Identity;

namespace _04_Domain.Entities.Profiles
{
    public class FreelancerProfile : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime BirthDate { get; set; }

        public int BusinessTypeId { get; set; }
        public BusinessType BusinessType { get; set; } // Tipo de Negócio: MEI, ME, SEM CNPJ

        public int ExperienceYearsId { get; set; }
        public ExperienceYears ExperienceYears { get; set; } // Tempo de Experiência: Menos de 1 ano, 1 a 3 anos, 3 a 5 anos, 5 a 10 anos, mais de 10 anos.

        public int WorkshopSizeId { get; set; }
        public WorkshopSize WorkshopSize { get; set; } // Tamanho da Oficina: Apenas eu, 2 a 3 pessoas, 4 a 6 pessoas, mais de 6 pessoas.

        public ICollection<FreelancerSpecialties> FreelancerSpecialties { get; set; } // Especialidades: Costura reta, Overloque, Modelagem, Peça piloto, Amostras, Acabamento, Bordado, Estamparia.
        public ICollection<FreelancerOwnMachines> FreelancerOwnMachines { get; set; } // Máquinas que possui: Reta, Overloque, Galoneira, Travete, Interlock, Máquina de corte.

        public int HowUsuallyArrangeServicesId { get; set; }
        public HowUsuallyArrangeServices HowUsuallyArrangeServices { get; set; } // Como costuma fechar serviços: Grupos whatsapp, indicação, redes sociais, plataformas online, outros (cite).

        public int AvailableTimeId { get; set; }
        public AvailableTime AvailableTime { get; set; } // Disponibilidade de Tempo: Período integral, meio período, fins de semana, sob demanda.

        public int FreelancerPreferencesId { get; set; }
        public FreelancerPreferences FreelancerPreferences { get; set; } // Preferências do Freelancer: Serviços pontuais, contrato fixo, grandes lotes, peças exclusivas/autorais.

        public int AverageRevenueId { get; set; }
        public AverageRevenue AverageRevenue { get; set; } // Faturamento médio atual: até R$ 1000, de R$ 1000 a R$ 3000 mil, de R$ 3000 A R$ 6000, acima de R$ 6000, prefiro não informar.

        public bool HasFixedProducer { get; set; } // Tem produtor fixo? Sim, Não
        public bool HasOwnCar { get; set; } // Tem veículo para buscar/levar peças? Sim, Não
    }
}