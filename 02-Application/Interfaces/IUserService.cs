using _02_Application.DTOs;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
using _02_Application.Enums;
using _04_Domain.Entities.Identity;

namespace _02_Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        string GetLoggedUserEmailAddress();

        Task<CreateUserResult> CreateFreelancerAsync(CreateFreelancerDto dto);
        Task<CreateUserResult> CreateCompanyAsync(CreateCompanyDto dto);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<int?> GetUserIdByEmailAsync(string email);
        Task<PagedResult<UserDto>> GetAllUsersAsync(int page, int pageSize);
        Task<PagedResult<GetFreelancerDto>> GetAllFreelancersAsync(int skip, int take);
        Task<GetFreelancerDto?> GetFreelancerProfileByIdAsync(int userId);
        Task<FreelancerPublicProfileDto?> GetFreelancerPublicProfileByIdAsync(int userId);
        Task<GetCompanyDto?> GetCompanyProfileByIdAsync(int userId);
        Task<CompanyPublicProfileDto?> GetCompanyPublicProfileByIdAsync(int userId);
        Task<ProfileUpdateResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto, int userId);
        Task<ProfileUpdateResult> UpdateUserCompanyAsync(UpdateCompanyDto dto, int userId);
        Task<bool> DeleteUserByIdAsync(int id);
        Task<AuthenticatedUserDto?> AuthenticateAsync(string email, string password);
    }
}