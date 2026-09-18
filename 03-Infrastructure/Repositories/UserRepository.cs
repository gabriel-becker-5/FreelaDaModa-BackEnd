using _03_Infrastructure.Data;
using _04_Domain.Entities.Identity;
using _04_Domain.Entities.Profiles;
using _04_Domain.Enums;
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

        public async Task<int?> CreateFreelancerUserProfileAsync(User user, FreelancerProfile profile)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                profile.UserId = user.Id;
                _context.FreelancerProfiles.Add(profile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return user.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<int?> CreateCompanyUserProfileAsync(User user, CompanyProfile profile)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                profile.UserId = user.Id;
                _context.CompanyProfiles.Add(profile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return user.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<ICollection<User>> GetAllUsersAsync(int skip, int take)
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountUsersAsync()
        {
            return await _context.Users.AsNoTracking().CountAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            // ATENÇÃO: não alterar para '.AsNoTracking' — A entidade é mutada por update/delete do UserService
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<User?> GetFreelancerProfileAsync(int id)
        {
            // ATENÇÃO: não alterar para '.AsNoTracking' — A entidade é mutada por update/delete do UserService
            return await _context.Users
                .Include(x => x.FreelancerProfile)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetCompanyProfileAsync(int id)
        {
            // ATENÇÃO: não alterar para '.AsNoTracking' — A entidade é mutada por update/delete do UserService
            return await _context.Users
                .Include(x => x.CompanyProfile)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ICollection<Roles>> GetUserRolesAsync(int userId)
        {
            User? result = await _context.Users.FindAsync(userId);

            if (result == null)
            {
                return [];
            }

            return result.Roles;
        }

        public async Task UpdateFreelancerAsync(User user, FreelancerProfile profile)
        {
            _context.FreelancerProfiles.Update(profile);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCompanyAsync(User user, CompanyProfile profile)
        {
            _context.CompanyProfiles.Update(profile);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        private bool IsUserRoleActive(User user, int roleId)
        {
            return user.Roles.Contains((Roles)roleId);
        }

        public async Task AddRoleToUserAsync(User user, int roleId)
        {
            if (!IsUserRoleActive(user, roleId))
            {
                user.Roles.Add((Roles)roleId);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveRoleFromUserAsync(User user, int roleId)
        {
            if (IsUserRoleActive(user, roleId))
            {
                user.Roles.Remove((Roles)roleId);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllRolesFromUserAsync(int userId)
        {
            User? result = await _context.Users.FindAsync(userId);

            if (result != null)
            {
                result.Roles.Clear();
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCurrentUserAsync(User user)
        {
            user.IsDeleted = true;

            CompanyProfile? company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == user.Id);
            if (company != null) company.IsDeleted = true;

            FreelancerProfile? freelancer = await _context.FreelancerProfiles
                .FirstOrDefaultAsync(fp => fp.UserId == user.Id);
            if (freelancer != null) freelancer.IsDeleted = true;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsEmailRegistered(string email)
        {
            User? result = await _context.Users.Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> IsCpfRegistered(string cpf)
        {
            return await _context.Users
                .AnyAsync(u => u.LegalResponsibleDocument.ToLower() == cpf.ToLower());
        }

        public async Task<bool> IsCnpjRegistered(string cnpj)
        {
            return await _context.CompanyProfiles
                .AnyAsync(c => c.CompanyRegistrationDocument.ToLower() == cnpj.ToLower());
        }
    }
}