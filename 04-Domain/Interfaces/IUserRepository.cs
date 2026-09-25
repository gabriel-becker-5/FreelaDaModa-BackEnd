using _04_Domain.Entities.Identity;
using _04_Domain.Entities.Profiles;
using _04_Domain.Enums;

namespace _04_Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<int?> CreateFreelancerUserProfileAsync(User user, FreelancerProfile profile);
        Task<int?> CreateCompanyUserProfileAsync(User user, CompanyProfile profile);
        Task<int> CountUsersAsync();
        Task<ICollection<User>> GetAllUsersAsync(int skip, int take);
        Task<ICollection<User>> GetAllFreelancersAsync(int skip, int take);
        Task<int> CountFreelancersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetFreelancerProfileAsync(int userId);
        Task<User?> GetCompanyProfileAsync(int userId);
        Task<ICollection<Roles>> GetUserRolesAsync(int userId);
        Task UpdateFreelancerAsync(User user, FreelancerProfile profile);
        Task UpdateCompanyAsync(User user, CompanyProfile profile);
        Task AddRoleToUserAsync(User user, int roleId);
        Task RemoveRoleFromUserAsync(User user, int roleId);
        Task RemoveAllRolesFromUserAsync(int userId);
        Task DeleteCurrentUserAsync(User user);
        Task<bool> IsEmailRegistered(string email);
        Task<bool> IsCpfRegistered(string cpf);
        Task<bool> IsCnpjRegistered(string cnpj);

        Task<string?> GetProfileImageKeyAsync(int userId);
        Task<bool> TrySetProfileImageKeyAsync(int userId, string? expectedKey, string? newKey);
    }
}