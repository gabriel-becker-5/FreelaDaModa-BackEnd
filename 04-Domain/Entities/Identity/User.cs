using _04_Domain.Enums;
using _04_Domain.Entities.Profiles;

namespace _04_Domain.Entities.Identity
{
    public class User : BaseEntity
    {
        public string LegalResponsibleFullName { get; set; }
        public string LegalResponsibleDocument { get; set; } // CPF
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ContactNumber { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Roles> Roles { get; set; }
        public string? PublicProfileDescription { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public int AddressNumber { get; set; }
        public string Neighborhood { get; set; }
        public string? AdditionalAddressInfo { get; set; } // Complemento
        public string City { get; set; }
        public string State { get; set; }
        public virtual FreelancerProfile FreelancerProfile { get; set; }
        public virtual CompanyProfile CompanyProfile { get; set; }
    }
}