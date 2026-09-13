using _04_Domain.Entities;
using _04_Domain.Entities.ObjectsFields;
using _04_Domain.Entities.Profiles;
using _04_Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace _03_Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<CompanyProfile> CompaniesProfiles { get; set; }
        public DbSet<FreelancerProfile> FreelancersProfiles { get; set; }
        public DbSet<AvailableTime> AvailableTimes { get; set; }
        public DbSet<AverageRevenue> AverageRevenues { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<ExperienceYears> ExperienceYears { get; set; }
        public DbSet<FreelancerOwnMachines> FreelancerOwnMachines { get; set; }
        public DbSet<FreelancerSpecialties> FreelancerSpecialties { get; set; }
        public DbSet<FreelancerPreferences> FreelancerPreferences { get; set; }
        public DbSet<HowUsuallyArrangeServices> HowUsuallyArrangeServices { get; set; }
        public DbSet<OwnMachine> OwnMachines { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<WorkshopSize> WorkshopSizes { get; set; }

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

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FreelancerProfile>()
                .HasOne(p => p.BusinessType)
                .WithMany()
                .HasForeignKey(p => p.BusinessTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FreelancerProfile>()
                .HasOne(p => p.ExperienceYears)
                .WithMany()
                .HasForeignKey(p => p.ExperienceYearsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FreelancerProfile>()
                .HasOne(p => p.WorkshopSize)
                .WithMany()
                .HasForeignKey(p => p.WorkshopSizeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FreelancerProfile>()
                .HasOne(p => p.HowUsuallyArrangeServices)
                .WithMany()
                .HasForeignKey(p => p.HowUsuallyArrangeServicesId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FreelancerProfile>()
                .HasOne(p => p.AvailableTime)
                .WithMany()
                .HasForeignKey(p => p.AvailableTimeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FreelancerProfile>()
                .HasOne(p => p.FreelancerPreferences)
                .WithMany()
                .HasForeignKey(p => p.FreelancerPreferencesId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FreelancerProfile>()
                .HasOne(p => p.AverageRevenue)
                .WithMany()
                .HasForeignKey(p => p.AverageRevenueId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}