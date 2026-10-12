using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using _03_Infrastructure.Data;
using _03_Infrastructure.Repositories;
using _03_Infrastructure.Repositories.Vaga;
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
                ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não configurado.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            // Registo de Repositórios e Serviços de Infraestrutura
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVagaRepository, VagaRepository>();
            services.AddScoped<ICandidaturaRepository, CandidaturaRepository>();
            services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}