using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUserRepository _userRepository;

        public RoleService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ICollection<IdLabelDto>> GetAllRolesAsync()
        {
            List<IdLabelDto> rolesDto = [];

            foreach (Roles role in Enum.GetValues<Roles>())
            {
                var dto = new IdLabelDto
                {
                    Id = (int)role,
                    Label = role.ToString().Replace("_", " ")
                };

                rolesDto.Add(dto);
            }

            return rolesDto;
        }

        public async Task<ICollection<IdLabelDto>> GetUserRolesAsync(int userId)
        {
            ICollection<Roles> userRoles = await _userRepository.GetUserRolesAsync(userId);

            return userRoles
                .Select(role => new IdLabelDto
                {
                    Id = (int)role,
                    Label = role.ToString().Replace("_", " ")
                })
                .ToList();
        }

        public async Task<bool> AddRoleToUserAsync(int userId, int roleId)
        {
            if (!Enum.IsDefined(typeof(Roles), roleId))
            {
                return false;
            }

            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            await _userRepository.AddRoleToUserAsync(user, roleId);
            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            if (!Enum.IsDefined(typeof(Roles), roleId))
            {
                return false;
            }

            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            await _userRepository.RemoveRoleFromUserAsync(user, roleId);
            return true;
        }

        public async Task<bool> RemoveAllRolesFromUserAsync(int userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            await _userRepository.RemoveAllRolesFromUserAsync(userId);
            return true;
        }
    }
}
