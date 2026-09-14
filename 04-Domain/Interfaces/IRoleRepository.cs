using _04_Domain.Entities.Identity;

namespace _04_Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> CreateRoleAsync(Role newRole);

        Task<ICollection<Role?>> GetAllRolesAsync();

        Task<Role?> GetRoleAsync(string roleName);

        Task<Role?> GetRoleByIdAsync(int id);

        Task<string?> GetRoleNameByIdAsync(int id);

        Task<bool> RoleExists(string roleName);

        Task UpdateRoleAsync(Role role);
        Task<bool?> DeleteRoleAsync(int id);

    }
}