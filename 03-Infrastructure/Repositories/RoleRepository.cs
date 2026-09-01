using _03_Infrastructure.Data;
using _04_Domain.Entities.UserInfo;
using _04_Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _03_Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role> CreateRoleAsync(Role newRole)
        {
            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();
            return (newRole);
        }

        public async Task<List<Role?>?> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> RoleExists(string roleName)
        {
            Role? result = await _context.Roles.Where(r => r.RoleName == roleName).FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<Role?> GetRoleAsync(string roleName)
        {
            Role? result = await _context.Roles.Where(r => r.RoleName == roleName).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            Role? result = await _context.Roles.FindAsync(id);

            if (result == null)
            {
                return null;
            }
            return result;
        }

        public async Task<string?> GetRoleNameByIdAsync(int id)
        {
            Role? result = await _context.Roles.FindAsync(id);

            if (result != null)
            {
                return result.RoleName.ToString();
            }

            return null;
        }
    }
}