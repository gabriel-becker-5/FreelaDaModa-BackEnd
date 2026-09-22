using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities.UserInfo;
using _04_Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace _02_Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
     IUserRepository userRepository,
     IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        PasswordHasher<User> passwordHasher = new();

        public async Task<User?> CreateUserAsync(UserRegisterDto dto)
        {
            if (await _userRepository.IsUserEmailRegistered(dto.Email))
            {
                return null;
            }

            User newUser = new()
            {
                Name = dto.Name,
                Email = dto.Email,
                BirthDate = dto.BirthDate
            };

            newUser.PasswordHash = passwordHasher.HashPassword(newUser, dto.Password);

            return await _userRepository.CreateUserAsync(newUser);
        }

        public async Task<bool> IsUserEmailRegistered(string email)
        {
            return await _userRepository.IsUserEmailRegistered(email);
        }

        public async Task<UserRole?> CreateUserRoleAsync(User user, Role role)
        {
            bool result = await _userRepository.UserRoleExists(user.Id, role.Id);

            if (result)
            {
                return null;
            }

            UserRole newUserRole = new()
            {
                Role = role,
                User = user
            };

            return await _userRepository.CreateUserRoleAsync(newUserRole);
        }

        public async Task RemoveRoleFromUserAsync(User user, Role role)
        {
            await _userRepository.RemoveRoleFromUserAsync(user.Id, role.Id);
        }

        public async Task RemoveAllRolesFromUserAsync(User user)
        {
            await _userRepository.RemoveAllRolesFromUserAsync(user.Id);
        }

        public string GetLoggedUserEmailAddress()
        {
            return _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.Name)?
                .Value;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            User? result = await _userRepository.GetByEmailAsync(email);

            if (result != null)
            {
                return result;
            }

            return null;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public PasswordVerificationResult VerifyPassword(User user, string senhaDto)
        {
            return passwordHasher.VerifyHashedPassword(user, user.PasswordHash, senhaDto);
        }

        public async Task<List<int>> GetUserRolesAsync(User user)
        {
            List<UserRole?> allUserRoles = await _userRepository.GetUserRoles(user.Id);

            List<int> rolesInteger = [];

            foreach (UserRole role in allUserRoles)
            {
                rolesInteger.Add(role.RoleId);
            }

            return rolesInteger;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.ListAllAsync();
        }

        public async Task UpdateUserAsync(UserUpdateDto dto, User user)
        {
            user.Email = dto.Email;
            user.Name = dto.Name;
            user.BirthDate = dto.BirthDate;
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            await _userRepository.DeleteUserAsync(user);
        }
    }
}