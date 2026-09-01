using _03_Infrastructure.Data;
using _04_Domain.Entities.UserInfo;
using _04_Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _03_Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> ListAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            User? result = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool> IsUserEmailRegistered(string email)
        {
            User? result = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserRole> CreateUserRoleAsync(UserRole userRole)
        {
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            UserRole? result = await _context.UserRoles.Where(ur => ur.UserId == userId &&
                                                                 ur.RoleId == roleId).
                                                                 FirstOrDefaultAsync();
            if (result != null)
            {
                _context.UserRoles.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllRolesFromUserAsync(int userId)
        {
            List<UserRole> allUserRoles = await _context.UserRoles.Where(ur => ur.UserId == userId)
                                                          .ToListAsync();

            foreach (UserRole userRole in allUserRoles)
            {
                _context.UserRoles.Remove(userRole);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserRoleExists(int userId, int roleId)
        {
            UserRole? result = await _context.UserRoles.Where(ur => ur.UserId == userId &&
                                                                 ur.RoleId == roleId).
                                                                 FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<List<UserRole?>> GetUserRoles(int userId)
        {
            List<UserRole> allUserRoles = await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
            return allUserRoles;
        }
    }
}