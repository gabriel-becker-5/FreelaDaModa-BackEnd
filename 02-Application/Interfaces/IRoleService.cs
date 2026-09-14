using _02_Application.DTOs.User;
using _04_Domain.Entities.Identity;

namespace _02_Application.Interfaces
{
    public interface IRoleService
    {
        Task<UserRoleDto?> CreateRoleAsync(string roleName);

        Task<ICollection<UserRoleDto>> GetAllRolesAsync();

        Task<int?> GetRoleIdByNameAsync(string roleName);

        Task<UserRoleDto?> GetRoleByIdAsync(int id);

        Task<ICollection<string>> GetRoleNameByIdAsync(ICollection<int> RolesIds);

        Task<bool?> UpdateRoleAsync(int id, string newRoleName);

        Task<bool?> DeleteRoleAsync(int id);
    }
}