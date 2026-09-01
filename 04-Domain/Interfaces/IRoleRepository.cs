using _04_Domain.Entities.UserInfo;

namespace _04_Domain.Interfaces
{
    public interface IRoleRepository
    {
        public Task<Role> CreateRoleAsync(Role newRole);

        public Task<List<Role?>?> GetAllRolesAsync();

        public Task<bool> RoleExists(string roleName);

        public Task<Role?> GetRoleAsync(string roleName);

        public Task<Role?> GetRoleByIdAsync(int id);

        public Task<string?> GetRoleNameByIdAsync(int id);
    }
}