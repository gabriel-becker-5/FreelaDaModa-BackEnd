using _02_Application.DTOs;
using _04_Domain.Entities.UserInfo;
using Microsoft.AspNetCore.Identity;

namespace _02_Application.Interfaces
{
    public interface IUserService
    {
        public Task<User> CreateUserAsync(UserRegisterDto dto);
        public Task<bool> IsUserEmailRegistered(string email);
        public Task<UserRole?> CreateUserRoleAsync(User user, Role role);
        public Task RemoveRoleFromUserAsync(User user, Role role);
        public Task RemoveAllRolesFromUserAsync(User user);
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<User?> GetUserByIdAsync(int id);
        public string GetLoggedUserEmailAddress();
        public PasswordVerificationResult VerifyPassword(User user, string passwordDto);
        public Task<List<int>> GetUserRolesAsync(User user);
        public Task<List<User>> GetAllUsersAsync();
        public Task UpdateUserAsync(UserUpdateDto dto, User user);
        public Task DeleteUserAsync(User user);
    }
}