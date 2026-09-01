using _02_Application.Interfaces;
using _04_Domain.Entities.UserInfo;
using _04_Domain.Interfaces;

namespace _02_Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository rolerepository)
        {
            _roleRepository = rolerepository;
        }

        public async Task<Role?> CreateRoleAsync(string roleName)
        {
            bool result = await _roleRepository.RoleExists(roleName);

            if (result)
            {
                return null;
            }

            Role newRole = new Role
            {
                RoleName = roleName
            };

            return await _roleRepository.CreateRoleAsync(newRole);
        }

        public async Task<List<Role?>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }

        public async Task<Role?> GetRoleAsync(string roleName)
        {
            Role? role = await _roleRepository.GetRoleAsync(roleName);

            if (role == null)
            {
                return null;
            }
            return role;
        }

        public async Task<Role?> GetRoleIdAsync(int id)
        {
            return await _roleRepository.GetRoleByIdAsync(id);
        }

        public async Task<List<string?>> GetRoleNameByIdAsync(List<int> RolesIds)
        {
            List<string> allUserRoles = [];

            foreach (int roleId in RolesIds)
            {
                string result = await _roleRepository.GetRoleNameByIdAsync(roleId);
                allUserRoles.Add(result);
            }

            return allUserRoles;
        }
    }
}