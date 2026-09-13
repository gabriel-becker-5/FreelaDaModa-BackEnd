using _02_Application.DTOs;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace _02_Application.Interfaces
{
    public interface IUserService
    {

        Task<bool> IsFreelancerFieldsValid(CreateFreelancerDto dto);

        Task<int?> CreateAdminUserAsync(UserDto newUser);

        Task<int?> RegisterFreelancerAsync(CreateFreelancerDto dto);

        Task<int?> RegisterCompanyAsync(CreateCompanyDto dto);


        Task<bool> CreateUserRoleAsync(int userId, int roleId);

        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<int?> GetUserIdByEmailAsync(string email);

        Task<UserDto?> GetUserByIdAsync(int id);

        Task<ICollection<UserDto>> GetAllUsersAsync();

        Task<GetFreelancerDto?> GetFreelancerProfileByEmailAsync(string userEmail);

        Task<GetCompanyDto?> GetCompanyProfileByEmailAsync(string userEmail);

        Task<ICollection<int>> GetUserRolesAsync(int id);


        Task<ProfileUpdateResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto, string userEmail);

        Task<ProfileUpdateResult> UpdateUserCompanyAsync(UpdateCompanyDto dto, string userEmail);

        Task RemoveRoleFromUserAsync(int userId, int roleId);

        Task RemoveAllRolesFromUserAsync(int id);

        Task<bool> DeleteCurrentUserAsync(string userEmail);
        Task<bool> DeleteUserByIdAsync(int id);

        Task<bool> IsUserEmailRegistered(string email);

        Task<PasswordVerificationResult> VerifyPassword(int userId, string password);
    }
}
