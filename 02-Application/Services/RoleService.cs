using _02_Application.Authorization;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
using _02_Application.Interfaces;
using _04_Domain.Entities.Identity;
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

        public bool IsSeedRole(string seedName)
        {
            if (seedName == Roles.Admin || seedName == Roles.Freelancer || seedName == Roles.Company)
            {
                return true; 
            }

            return false;
        }


        public async Task<UserRoleDto?> CreateRoleAsync(string roleName)
        {
            bool result = await _roleRepository.RoleExists(roleName);

            if (result)
            {
                return null;
            }

            await _roleRepository.CreateRoleAsync(new Role { RoleName = roleName });

            UserRoleDto dto = new()
            {
                RoleName = roleName
            };

            return dto;
        }

        public async Task<ICollection<UserRoleDto>> GetAllRolesAsync()
        {
            ICollection<Role?> roles = await _roleRepository.GetAllRolesAsync();

            List<UserRoleDto> rolesDto = [];

            foreach (Role role in roles)
            {
                UserRoleDto dto = new()
                {
                    RoleName = role.RoleName
                };

                rolesDto.Add(dto);
            }

            return rolesDto;
        }

        public async Task<int?> GetRoleIdByNameAsync(string roleName)
        {
            Role? role = await _roleRepository.GetRoleAsync(roleName);

            if (role == null)
            {
                return null;
            }

            return role.Id;
        }

        public async Task<UserRoleDto?> GetRoleByIdAsync(int id)
        {
            Role? role = await _roleRepository.GetRoleByIdAsync(id);

            if (role == null)
            {
                return null;
            }

            UserRoleDto dto = new()
            {
                RoleId = role.Id,
                RoleName = role.RoleName
            };

            return dto;
        }

        public async Task<ICollection<string>> GetRoleNameByIdAsync(ICollection<int> RolesIds)
        {
            List<string> allUserRoles = [];

            foreach (int roleId in RolesIds)
            {
                string result = await _roleRepository.GetRoleNameByIdAsync(roleId);

                if (result != null)
                {
                    allUserRoles.Add(result);
                }
            }

            return allUserRoles;
        }

        public async Task<bool?> UpdateRoleAsync(int id, string newRoleName)
        {
            Role? result = await _roleRepository.GetRoleByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            if (IsSeedRole(result.RoleName) || await _roleRepository.RoleExists(newRoleName))
            {
                return false;
            }

            if (result.RoleName != newRoleName)
            {
                result.RoleName = newRoleName;
                await _roleRepository.UpdateRoleAsync(result);
                return true;
            }

            return false;
        }

        public async Task<bool?> DeleteRoleAsync(int id)
        {
            string? result = await _roleRepository.GetRoleNameByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            if (IsSeedRole(result))
            {
                return false;
            }

            return await _roleRepository.DeleteRoleAsync(id);
        }
    }
}