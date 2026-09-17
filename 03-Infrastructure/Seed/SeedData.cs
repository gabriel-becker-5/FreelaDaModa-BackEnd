using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _03_Infrastructure.Seed
{
    public class SeedData
    {
        public async static Task Initializer(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            // Cria o usuário inicial de Admin
            User admin = new()
            {
                LegalResponsibleFullName = "Administrador do Sistema",
                LegalResponsibleDocument = "00000000000",
                Email = "admin@admin.com",
                ContactNumber = "0000000000",
                PostalCode = "00000000",
                Address = "N/A",
                AddressNumber = 0,
                Neighborhood = "N/A",
                City = "Blumenau",
                State = "SC",
                Roles = new List<Roles> { Roles.Admin },
                PasswordHash = passwordHasher.HashPassword("123")
            };

            User? existing = await userRepository.GetUserByEmailAsync(admin.Email);

            if (existing == null)
            {
                await userRepository.CreateUserAsync(admin);
            }
        }
    }
}
