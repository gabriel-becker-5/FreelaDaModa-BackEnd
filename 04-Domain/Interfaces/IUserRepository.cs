using _04_Domain.Entities.UserInfo;

namespace _04_Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> ListAllAsync();
        public Task<User?> GetByIdAsync(int id);
        public Task<User?> GetByEmailAsync(string email);
        public Task<bool> IsUserEmailRegistered(string email);
        public Task<User> CreateUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task DeleteUserAsync(User user);
        public Task<UserRole> CreateUserRoleAsync(UserRole userRole);
        public Task RemoveRoleFromUserAsync(int userId, int roleId);
        public Task RemoveAllRolesFromUserAsync(int userId);
        public Task<bool> UserRoleExists(int userId, int roleId);
        public Task<List<UserRole?>> GetUserRoles(int userId);
    }
}