using _04_Domain.Entities.ObjectsFields;
using _04_Domain.Entities.Profiles;
using _04_Domain.Entities.Identity;

namespace _04_Domain.Interfaces
{
    public interface IUserRepository
    {




        Task<User> CreateUserAsync(User user);
        Task<FreelancerProfile> CreateFreelancerProfileAsync(FreelancerProfile dto);

        Task<CompanyProfile> CreateCompanyProfileAsync(CompanyProfile dto);

        Task CreateFreelancerSpecialties(ICollection<FreelancerSpecialties> freelancerSpecialties);
        Task CreateFreelancerOwnMachines(ICollection<FreelancerOwnMachines> freelancerOwnMachines);


        Task<UserRole> CreateUserRoleAsync(UserRole userRole);
        Task<FreelancerSpecialties> AddFreelancerSpecialtyAsync(FreelancerSpecialties freelancerSpecialties);
        Task<FreelancerOwnMachines> AddFreelancerOwnMachineAsync(FreelancerOwnMachines freelancerOwnMachines);

        Task<ICollection<User>> GetAllUsersAsync(int skip, int take);
        Task<int> CountUsersAsync();

        Task<User?> GetUserByIdAsync(int id);

        Task<User?> GetUserByEmailAsync(string email);

        Task<FreelancerProfile?> GetFreelancerProfileAsync(int userId);

        Task<CompanyProfile?> GetCompanyProfileAsync(int userId);

        Task<ICollection<UserRole>> GetUserRolesAsync(int userId);


        Task UpdateFreelancerAsync(int freelancerId,
                                        ICollection<FreelancerSpecialties>? freelancerSpecialties,
                                        ICollection<FreelancerOwnMachines>? freelancerOwnMachines);

        Task UpdateCompanyAsync();

        Task RemoveAllFreelancerSpecialties(int freelancerId);

        Task RemoveAllFreelancerOwnMachines(int freelancerId);

        Task RemoveRoleFromUserAsync(int userId, int roleId);

        Task RemoveAllRolesFromUserAsync(int userId);

        Task DeleteCurrentUserAsync(User user);

        Task<bool> IsUserEmailRegistered(string email);

        Task<bool> UserRoleExists(int userId, int roleId);


    }
}