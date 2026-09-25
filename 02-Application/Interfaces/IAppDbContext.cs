using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;

namespace _02_Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Vaga> Vagas { get; }
        DbSet<Candidatura> Candidaturas { get; }
        DbSet<CompanyProfile> CompanyProfiles { get; }
        DbSet<FreelancerProfile> FreelancerProfiles { get; }
        DbSet<Notificacao> Notificacoes { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}