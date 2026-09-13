namespace _02_Application.DTOs.User
{
    public class UserDto
    {
        public string LegalResponsibleFullName { get; set; }
        public string LegalResponsibleDocument { get; set; } // CPF
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string PublicProfileDescription { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public int AddressNumber { get; set; }
        public string Quarter { get; set; }
        public string? AdditionalAddressInfo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}