using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using _03_Infrastructure.Data;
using _03_Infrastructure.Repositories;
using _03_Infrastructure.Services;
using _04_Domain.Interfaces;
using _02_Application.Interfaces; // Caso o ITokenService esteja aqui ou na Infra, ajusta o using se necessário

namespace _03_Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost;Port=3306;Database=freeladamoda;User=root;Password=root;";

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.36-mysql"))
            );

            // Registo de Repositórios e Serviços de Infraestrutura
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();
            services.AddScoped<ITokenService, TokenService>(); // <-- Adicionado aqui para resolver o erro do AuthenticationController

            return services;
        }
    }
}