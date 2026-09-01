using _04_Domain.Entities.UserInfo;

namespace _02_Application.Interfaces
{
    public interface IRoleService
    {
        public Task<Role?> CreateRoleAsync(string roleName);

        public Task<List<Role?>?> GetAllRolesAsync();

        public Task<Role?> GetRoleAsync(string roleName);

        public Task<Role?> GetRoleIdAsync(int id);

        public Task<List<string?>> GetRoleNameByIdAsync(List<int> RolesIds);
    }
}