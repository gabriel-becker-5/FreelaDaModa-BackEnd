using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using _04_Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace _03_Infrastructure.Seed
{
    public class SeedData
    {
        public async static Task Initializer(IUserRepository userRepository,
                                             IPasswordHasher passwordHasher,
                                             IConfiguration configuration)
        {
            string adminEmail = configuration["AdminSeed:Email"]
                ?? throw new InvalidOperationException("AdminSeed:Email não configurado.");

            string adminPassword = configuration["AdminSeed:Password"]
                ?? throw new InvalidOperationException("AdminSeed:Password não configurado.");

            // Cria o usuário inicial de Admin
            User admin = new()
            {
                LegalResponsibleFullName = "Administrador do Sistema",
                LegalResponsibleDocument = "00000000000",
                Email = adminEmail,
                ContactNumber = "0000000000",
                PostalCode = "00000000",
                Address = "N/A",
                AddressNumber = 0,
                Neighborhood = "N/A",
                City = "N/A",
                State = "N/A",
                Roles = new List<Roles> { Roles.Admin },
                PasswordHash = passwordHasher.HashPassword(adminPassword),
                PublicProfileDescription = "N/A"
            };

            User? existing = await userRepository.GetUserByEmailAsync(adminEmail);

            if (existing == null)
            {
                await userRepository.CreateUserAsync(admin);
            }
        }
    }
}
