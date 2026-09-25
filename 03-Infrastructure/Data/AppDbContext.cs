using _04_Domain.Entities;
<<<<<<< HEAD
using _04_Domain.Entities.UserInfo;
=======
using _04_Domain.Entities.Identity;
using _04_Domain.Entities.Profiles;
using _04_Domain.Enums;
>>>>>>> origin/main
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace _03_Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
<<<<<<< HEAD
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<OrdemServico> OrdensServico { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }
=======
        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<FreelancerProfile> FreelancerProfiles { get; set; }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SetAuditProperties();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditProperties()
        {
            IEnumerable<EntityEntry<BaseEntity>> entries = ChangeTracker
                .Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Property(x => x.CreatedAt).IsModified = false;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(u =>
            {
                u.HasQueryFilter(x => !x.IsDeleted);
                u.Property(x => x.LegalResponsibleFullName).HasMaxLength(100);
                u.Property(x => x.LegalResponsibleDocument).HasMaxLength(14);
                u.Property(x => x.Email).HasMaxLength(100);
                u.Property(x => x.PasswordHash).HasMaxLength(512);
                u.Property(x => x.ContactNumber).HasMaxLength(11);
                u.Property(x => x.PublicProfileDescription).HasMaxLength(500);
                u.Property(x => x.PostalCode).HasMaxLength(9);
                u.Property(x => x.Address).HasMaxLength(150);
                u.Property(x => x.Neighborhood).HasMaxLength(100);
                u.Property(x => x.AdditionalAddressInfo).HasMaxLength(150);
                u.Property(x => x.City).HasMaxLength(150);
                u.Property(x => x.State).HasMaxLength(150);
                u.Property(x => x.ProfileImageKey).HasColumnType("varchar(255)").IsRequired(false);

                u.Property(x => x.Roles)
                    .HasColumnType("json")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<Roles>>(v, (JsonSerializerOptions?)null)
                    );

                u.HasIndex(x => x.Email);
                u.HasIndex(x => x.LegalResponsibleDocument);
            });

            modelBuilder.Entity<CompanyProfile>(c =>
            {
                c.HasQueryFilter(x => !x.IsDeleted);
                c.Property(x => x.LegalName).HasMaxLength(150);
                c.Property(x => x.CompanyName).HasMaxLength(150);
                c.Property(x => x.CompanyRegistrationDocument).HasMaxLength(14);
                c.Property(x => x.CoreBusiness).HasMaxLength(150);

                c.HasIndex(x => x.CompanyRegistrationDocument);
            });

            modelBuilder.Entity<FreelancerProfile>(f =>
            {
                f.HasQueryFilter(x => !x.IsDeleted);
                f.Property(x => x.SpecialtiesIds)
                    .HasColumnType("json")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<Specialty>>(v, (JsonSerializerOptions?)null)
                    );

                f.Property(x => x.OwnMachinesIds)
                    .HasColumnType("json")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<OwnMachine>>(v, (JsonSerializerOptions?)null)
                    );
            });
        }
>>>>>>> origin/main
    }
}