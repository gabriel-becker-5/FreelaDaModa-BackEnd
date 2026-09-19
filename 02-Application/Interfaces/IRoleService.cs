using _02_Application.DTOs;

namespace _02_Application.Interfaces
{
    public interface IRoleService
    {
        Task<ICollection<IdLabelDto>> GetAllRolesAsync();
        Task<ICollection<IdLabelDto>> GetUserRolesAsync(int userId);
        Task<bool> AddRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<bool> RemoveAllRolesFromUserAsync(int userId);
    }
}
