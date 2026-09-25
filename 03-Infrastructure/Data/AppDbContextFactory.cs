using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace _03_Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = "Server=localhost;Port=3306;Database=freeladamoda;User=root;Password=root;";

            optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.36-mysql"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
