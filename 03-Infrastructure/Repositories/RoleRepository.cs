using _03_Infrastructure.Data;
using _04_Domain.Entities.Identity;
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
            _context.Add(newRole);
            await _context.SaveChangesAsync();
            return (newRole);
        }

        public async Task<ICollection<Role?>> GetAllRolesAsync()
        {
            return await _context.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<Role?> GetRoleAsync(string roleName)
        {
            Role? result = await _context.Roles.Where(r => r.RoleName == roleName).AsNoTracking().FirstOrDefaultAsync();

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
            Role? result = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

            if (result != null)
            {
                return result.RoleName.ToString();
            }

            return null;
        }

        public async Task<bool> RoleExists(string roleName)
        {
            Role? result = await _context.Roles.Where(r => r.RoleName == roleName).AsNoTracking().FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }


        public async Task UpdateRoleAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task<bool?> DeleteRoleAsync(int id)
        {
            Role? result = await _context.Roles.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}