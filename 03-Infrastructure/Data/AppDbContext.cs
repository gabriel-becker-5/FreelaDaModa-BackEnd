using _04_Domain.Entities;
using _04_Domain.Entities.UserInfo;
using Microsoft.EntityFrameworkCore;

namespace _03_Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<OrdemServico> OrdensServico { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }
    }
}