using _04_Domain.Entities.Identity;

namespace _04_Domain.Entities.Profiles
{
    public class CompanyProfile : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string LegalName { get; set; } // Razão Social
        public string CompanyName { get; set; } // Nome Fantasia
        public string CompanyRegistrationDocument { get; set; } // CNPJ
        public string CoreBusiness { get; set; } // Ramo de Atuação
    }
}