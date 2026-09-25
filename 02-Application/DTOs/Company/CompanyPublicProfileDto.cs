namespace _02_Application.DTOs.Company
{
    public class CompanyPublicProfileDto
    {
        public string CompanyName { get; set; } // Nome Fantasia
        public string City { get; set; }
        public string State { get; set; }
        public bool IsVerified { get; set; } // Perfil Verificado
        public string PublicProfileDescription { get; set; }
        public string CoreBusiness { get; set; } // Ramo de Atuação

        public string? ProfileImageUrl { get; set; }
    }
}