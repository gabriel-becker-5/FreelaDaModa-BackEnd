using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using _03_Infrastructure.Data;
using _03_Infrastructure.Repositories;
using _03_Infrastructure.Repositories.Vaga; // <-- Namespace para os repositórios de Vaga
using _03_Infrastructure.Services;
using _04_Domain.Interfaces;
using _02_Application.Interfaces;

namespace _03_Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost;Port=3306;Database=freeladamoda;User=root;Password=root;";

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            // Registo de Repositórios e Serviços de Infraestrutura
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVagaRepository, VagaRepository>(); // <-- Adicionado para resolver a dependência da vaga
            services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}