using _03_Infrastructure.Data;
using _04_Domain.Entities.ObjectsFields;
using _04_Domain.Entities.Profiles;
using _04_Domain.Entities.Identity;
using _04_Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _03_Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Crud Usuários

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Crud Profile/Perfil

        public async Task<FreelancerProfile> CreateFreelancerProfileAsync(FreelancerProfile profile)
        {
            _context.FreelancersProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<CompanyProfile> CreateCompanyProfileAsync(CompanyProfile profile)
        {
            _context.CompaniesProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task CreateFreelancerSpecialties(ICollection<FreelancerSpecialties> freelancerSpecialties)
        {
            foreach (var specialty in freelancerSpecialties)
            {
                _context.FreelancerSpecialties.Add(specialty);
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateFreelancerOwnMachines(ICollection<FreelancerOwnMachines> freelancerOwnMachines)
        {
            foreach (var ownMachine in freelancerOwnMachines)
            {
                _context.FreelancerOwnMachines.Add(ownMachine);
            }

            await _context.SaveChangesAsync();
        }



        // Role do Usuário

        public async Task<UserRole> CreateUserRoleAsync(UserRole userRole)
        {
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        // Especialidades do Freelancer

        public async Task<FreelancerSpecialties> AddFreelancerSpecialtyAsync(FreelancerSpecialties freelancerSpecialties)
        {
            _context.FreelancerSpecialties.Add(freelancerSpecialties);
            await _context.SaveChangesAsync();
            return freelancerSpecialties;
        }

        // Máquinas do Freelancer

        public async Task<FreelancerOwnMachines> AddFreelancerOwnMachineAsync(FreelancerOwnMachines freelancerOwnMachines)
        {
            _context.FreelancerOwnMachines.Add(freelancerOwnMachines);
            await _context.SaveChangesAsync();
            return freelancerOwnMachines;
        }

        public async Task<ICollection<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // ATENÇÃO: sem AsNoTracking de propósito — entidade mutada pelos fluxos de update/delete do UserService.
            User? result = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<FreelancerProfile?> GetFreelancerProfileAsync(int userId)
        {
            // ATENÇÃO: sem AsNoTracking de propósito — entidade mutada pelos fluxos de update/delete do UserService.
            FreelancerProfile? result = await _context.FreelancersProfiles.Where(fp => fp.UserId == userId).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }


        public async Task<CompanyProfile?> GetCompanyProfileAsync(int userId)
        {
            // ATENÇÃO: sem AsNoTracking de propósito — entidade mutada pelos fluxos de update/delete do UserService.
            CompanyProfile? result = await _context.CompaniesProfiles.Where(cp => cp.UserId == userId).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<ICollection<UserRole>> GetUserRolesAsync(int userId)
        {
            List<UserRole> allUserRoles = await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
            return allUserRoles;
        }


        public async Task UpdateFreelancerAsync(int freelancerId,
                                        ICollection<FreelancerSpecialties>? freelancerSpecialties,
                                        ICollection<FreelancerOwnMachines>? freelancerOwnMachines)
        {
            if (freelancerSpecialties?.Count > 0)
            {
                await RemoveAllFreelancerSpecialties(freelancerId);

                foreach (FreelancerSpecialties specialty in freelancerSpecialties)
                {
                    await AddFreelancerSpecialtyAsync(specialty);
                }

            }

            if (freelancerOwnMachines?.Count > 0)
            {
                await RemoveAllFreelancerOwnMachines(freelancerId);

                foreach (FreelancerOwnMachines ownMachine in freelancerOwnMachines)
                {
                    await AddFreelancerOwnMachineAsync(ownMachine);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCompanyAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAllFreelancerSpecialties(int freelancerId)
        {
            List<FreelancerSpecialties> allSpecialties = await _context.FreelancerSpecialties.Where(fs => fs.FreelancerId == freelancerId).ToListAsync();

            foreach (var specialty in allSpecialties)
            {
                _context.FreelancerSpecialties.Remove(specialty);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveAllFreelancerOwnMachines(int freelancerId)
        {
            List<FreelancerOwnMachines> allOwnMachines = await _context.FreelancerOwnMachines.Where(fom => fom.FreelancerId == freelancerId).ToListAsync();

            foreach (var ownMachine in allOwnMachines)
            {
                _context.FreelancerOwnMachines.Remove(ownMachine);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            UserRole? result = await _context.UserRoles.Where(ur => ur.UserId == userId &&
                                                                 ur.RoleId == roleId).
                                                                 FirstOrDefaultAsync();
            if (result != null)
            {
                _context.UserRoles.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllRolesFromUserAsync(int userId)
        {
            List<UserRole> allUserRoles = await _context.UserRoles.Where(ur => ur.UserId == userId)
                                                          .ToListAsync();

            foreach (UserRole userRole in allUserRoles)
            {
                _context.UserRoles.Remove(userRole);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCurrentUserAsync(User user)
        {
            user.IsDeleted = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserEmailRegistered(string email)
        {
            User? result = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UserRoleExists(int userId, int roleId)
        {
            UserRole? result = await _context.UserRoles.Where(ur => ur.UserId == userId &&
                                                                 ur.RoleId == roleId).
                                                                 FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }
    }
}