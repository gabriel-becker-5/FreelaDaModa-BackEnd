using _02_Application.DTOs.User;

namespace _02_Application.DTOs.Company
{
    public class GetCompanyDto : UserDto
    {
        // Campos de Empresa/Confecção
        public string LegalName { get; set; } // Razão Social
        public string CompanyName { get; set; } // Nome Fantasia
        public string CompanyRegistrationDocument { get; set; } // CNPJ
        public string CoreBusiness { get; set; } // Ramo de Atuação
    }
}