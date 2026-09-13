using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _04_Domain.Entities.ObjectsFields;
using _04_Domain.Entities.Profiles;
using _04_Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using _02_Application.DTOs.User;

namespace _02_Application.Interfaces
{
    public interface IUserService
    {


        Task<bool> IsFreelancerFieldsValid(CreateFreelancerDto dto);

        Task<int?> CreateAdminUserAsync(UserDto newUser);

        Task<int?> CreateFreelancerAsync(CreateFreelancerDto dto);

        Task<int?> CreateCompanyAsync(CreateCompanyDto dto);


        public Task<bool> CreateUserRoleAsync(int userId, int roleId);

        public Task<UserDto?> GetUserByEmailAsync(string email);
        public Task<int?> GetUserIdByEmailAsync(string email);

        Task<UserDto?> GetUserByIdAsync(int id);

        Task<ICollection<UserDto>> GetAllUsersAsync();


        Task<FreelancerProfile?> GetFreelancerProfileAsync(int userId);


        Task<CompanyProfile?> GetCompanyProfileAsync(int userId);

        public Task<ICollection<int>> GetUserRolesAsync(int id);


        public Task<bool> UpdateUserFreelancerAsync(UpdateFreelancerDto dto, UserDto user, FreelancerProfile profile);

        Task<bool> UpdateUserCompanyAsync(UpdateCompanyDto dto, UserDto user, CompanyProfile profile);

        Task RemoveRoleFromUserAsync(int userId, int roleId);

        public Task RemoveAllRolesFromUserAsync(int id);

        Task DeleteCurrentUserAsync(User user);
        public Task<bool> DeleteUserByIdAsync(int id);

        Task<bool> IsUserEmailRegistered(string email);

        public Task<PasswordVerificationResult> VerifyPassword(int userId, string password);
    }
}