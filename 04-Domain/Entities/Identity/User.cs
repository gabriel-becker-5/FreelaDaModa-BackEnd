using _04_Domain.Entities.Profiles;
using _04_Domain.Enums;

namespace _04_Domain.Entities.Identity
{
    public class User : BaseEntity
    {
        public string LegalResponsibleFullName { get; set; } // Nome Completo (Freelancer) ou Nome do responsável legal (Confecção)
        public string LegalResponsibleDocument { get; set; } // CPF do Freelancer ou Responsável legal
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ContactNumber { get; set; }
        public bool IsDeleted { get; set; } = false; // Usuário ativo ou não (Soft-delete)
        public bool IsVerified { get; set; } = false; // Perfil Verificado
        public ICollection<Roles> Roles { get; set; }
        public string PublicProfileDescription { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public int AddressNumber { get; set; }
        public string Neighborhood { get; set; }
        public string? AdditionalAddressInfo { get; set; } // Complemento, não obrigatório
        public string City { get; set; }
        public string State { get; set; }
        public DateTime BirthDate { get; set; }
        public virtual FreelancerProfile FreelancerProfile { get; set; }
        public virtual CompanyProfile CompanyProfile { get; set; }
        public string? ProfileImageKey { get; set; }
    }
}