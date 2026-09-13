using _02_Application.Authorization;
using _02_Application.DTOs;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
using _02_Application.Interfaces;
using _04_Domain.Entities.ObjectsFields;
using _04_Domain.Entities.Profiles;
using _04_Domain.Entities.Identity;
using _04_Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace _02_Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFreelancerFieldsRepository _freelancerFieldsRepository;
        private readonly IRoleService _roleService;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserService(IUserRepository userRepository,
                           IFreelancerFieldsRepository freelancerFieldsRepository,
                           IRoleService roleService)
        {
            _userRepository = userRepository;
            _freelancerFieldsRepository = freelancerFieldsRepository;
            _roleService = roleService;
        }

        // Criação de usuário

        public async Task<int?> CreateAdminUserAsync(UserDto dto)
        {
            if (await _userRepository.IsUserEmailRegistered(dto.Email))
            {
                return null;
            }

            User newUser = new()
            {
                LegalResponsibleFullName = dto.LegalResponsibleFullName,
                LegalResponsibleDocument = dto.LegalResponsibleDocument,
                Address = dto.Address,
                AddressNumber = dto.AddressNumber ?? 0,
                Quarter = dto.Quarter,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                AdditionalAddressInfo = dto.AdditionalAddressInfo,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email,
                PublicProfileDescription = dto.PublicProfileDescription,
                PasswordHash = _passwordHasher.HashPassword(null, "123"),
            };

            User createdUser = await _userRepository.CreateUserAsync(newUser);
            return createdUser.Id;
        }


        public async Task<bool> IsFreelancerFieldsValid(CreateFreelancerDto dto)
        {
            if (!await _freelancerFieldsRepository.AvailableTimeExistsAsync(dto.AvailableTimeId))
            {
                return false;
            }

            if (!await _freelancerFieldsRepository.AverageRevenueExistsAsync(dto.AverageRevenueId))
            {
                return false;
            }

            if (!await _freelancerFieldsRepository.BusinessTypeExistsAsync(dto.BusinessTypeId))
            {
                return false;
            }

            if (!await _freelancerFieldsRepository.ExperienceYearsExistsAsync(dto.ExperienceYearsId))
            {
                return false;
            }

            if (!await _freelancerFieldsRepository.FreelancerPreferencesExistsAsync(dto.FreelancerPreferencesId))
            {
                return false;
            }

            if (!await _freelancerFieldsRepository.HowUsuallyArrangeServicesExistsAsync(dto.HowUsuallyArrangeServicesId))
            {
                return false;
            }

            if (!await _freelancerFieldsRepository.WorkshopSizeExistsAsync(dto.WorkshopSizeId))
            {
                return false;
            }

            if (!await _freelancerFieldsRepository.SpecialtyExistsAsync(dto.SpecialtyIds))
            {
                return false;
            }

            if (!await _freelancerFieldsRepository.OwnMachinesExistsAsync(dto.OwnMachineIds))
            {
                return false;
            }

            return true;
        }

        public async Task<int?> RegisterFreelancerAsync(CreateFreelancerDto dto)
        {
            if (await _userRepository.IsUserEmailRegistered(dto.Email) || !await IsFreelancerFieldsValid(dto))
            {
                return null;
            }

            int? roleId = await _roleService.GetRoleIdByNameAsync(Roles.Freelancer);
            if (roleId == null)
            {
                return null;
            }

            int? userId = await CreateFreelancerAsync(dto);
            if (userId == null)
            {
                return null;
            }

            bool userRoleCreated = await CreateUserRoleAsync((int)userId, (int)roleId);
            return userRoleCreated ? userId : null;
        }

        public async Task<int?> RegisterCompanyAsync(CreateCompanyDto dto)
        {
            if (await _userRepository.IsUserEmailRegistered(dto.Email))
            {
                return null;
            }

            int? roleId = await _roleService.GetRoleIdByNameAsync(Roles.Company);
            if (roleId == null)
            {
                return null;
            }

            int? userId = await CreateCompanyAsync(dto);
            if (userId == null)
            {
                return null;
            }

            bool userRoleCreated = await CreateUserRoleAsync((int)userId, (int)roleId);
            return userRoleCreated ? userId : null;
        }

        private async Task<int?> CreateFreelancerAsync(CreateFreelancerDto dto)
        {
            if (await _userRepository.IsUserEmailRegistered(dto.Email))
            {
                return null;
            }

            if (!await IsFreelancerFieldsValid(dto))
            {
                return null;
            }

            User newUser = new()
            {
                LegalResponsibleFullName = dto.LegalResponsibleFullName,
                LegalResponsibleDocument = dto.LegalResponsibleDocument,
                Email = dto.Email,
                ContactNumber = dto.ContactNumber,
                PublicProfileDescription = dto.PublicProfileDescription,
                Address = dto.Address,
                AddressNumber = dto.AddressNumber,
                Quarter = dto.Quarter,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                AdditionalAddressInfo = dto.AdditionalAddressInfo
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, dto.Password);
            User createdUser = await _userRepository.CreateUserAsync(newUser);

            FreelancerProfile newFreelancer = new()
            {
                UserId = createdUser.Id,
                BirthDate = dto.BirthDate,
                HasFixedProducer = dto.HasFixedProducer,
                HasOwnCar = dto.HasOwnCar,
                AvailableTimeId = dto.AvailableTimeId,
                AverageRevenueId = dto.AverageRevenueId,
                BusinessTypeId = dto.BusinessTypeId,
                ExperienceYearsId = dto.ExperienceYearsId,
                FreelancerPreferencesId = dto.FreelancerPreferencesId,
                HowUsuallyArrangeServicesId = dto.HowUsuallyArrangeServicesId,
                WorkshopSizeId = dto.WorkshopSizeId,
                FreelancerSpecialties = [],
                FreelancerOwnMachines = []
            };

            FreelancerProfile createdFreelancerProfile = await _userRepository.CreateFreelancerProfileAsync(newFreelancer);

            List<FreelancerSpecialties> freelancerSpecialties = [];

            foreach (int specialty in dto.SpecialtyIds)
            {
                freelancerSpecialties.Add(new()
                {
                    FreelancerId = createdFreelancerProfile.Id,
                    SpecialtyId = specialty
                });
            }

            await _userRepository.CreateFreelancerSpecialties(freelancerSpecialties);
            
            List<FreelancerOwnMachines> freelancerOwnMachines = [];

            foreach (int machine in dto.OwnMachineIds)
            {
                freelancerOwnMachines.Add(new()
                {
                    FreelancerId = createdFreelancerProfile.Id,
                    OwnMachineId = machine
                });
            }

            await _userRepository.CreateFreelancerOwnMachines(freelancerOwnMachines);

            return createdUser.Id;
        }

        private async Task<int?> CreateCompanyAsync(CreateCompanyDto dto)
        {
            if (await _userRepository.IsUserEmailRegistered(dto.Email))
            {
                return null;
            }

            User newUser = new()
            {
                LegalResponsibleFullName = dto.LegalResponsibleFullName,
                LegalResponsibleDocument = dto.LegalResponsibleDocument,
                Email = dto.Email,
                ContactNumber = dto.ContactNumber,
                PublicProfileDescription = dto.PublicProfileDescription,
                Address = dto.Address,
                AddressNumber = dto.AddressNumber,
                Quarter = dto.Quarter,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                AdditionalAddressInfo = dto.AdditionalAddressInfo
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, dto.Password);
            User createdUser = await _userRepository.CreateUserAsync(newUser);

            CompanyProfile newCompany = new()
            {
                UserId = createdUser.Id,
                CompanyName = dto.CompanyName,
                CoreBusiness = dto.CoreBusiness,
                CompanyRegistrationDocument = dto.CompanyRegistrationDocument,
                LegalName = dto.LegalName
            };

            await _userRepository.CreateCompanyProfileAsync(newCompany);

            return createdUser.Id;
        }


        // Roles do usuário
        public async Task<bool> CreateUserRoleAsync(int userId, int roleId)
        {
            bool result = await _userRepository.UserRoleExists(userId, roleId);
            
            if (result)
            {
                return false;
            }

            UserRole newUserRole = new()
            {
                RoleId = roleId,
                UserId = userId
            };

            await _userRepository.CreateUserRoleAsync(newUserRole);
            return true;
        }

        // Leitura de usuário

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            User? result = await _userRepository.GetUserByEmailAsync(email);

            if (result == null)
            {
                return null;
            }

            return MapToDto(result);
        }

        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            User? result = await _userRepository.GetUserByEmailAsync(email);

            if (result == null)
            {
                return null;
            }

            return result.Id;
        }



        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            User? result = await _userRepository.GetUserByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            return MapToDto(result);
        }

        public async Task<PagedResult<UserDto>> GetAllUsersAsync(int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            ICollection<User> users = await _userRepository.GetAllUsersAsync(skip, pageSize);
            int total = await _userRepository.CountUsersAsync();

            List<UserDto> items = [];

            foreach (User user in users)
            {
                items.Add(MapToDto(user));
            }

            return new PagedResult<UserDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        // Leitura de perfil

        public async Task<GetFreelancerDto?> GetFreelancerProfileByEmailAsync(string userEmail)
        {
            User? user = await _userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return null;
            }

            FreelancerProfile? profile = await _userRepository.GetFreelancerProfileAsync(user.Id);
            if (profile == null)
            {
                return null;
            }

            FreelancerProfileDto? fields = await _freelancerFieldsRepository.GetProfileFieldsNames(profile.Id);
            if (fields == null)
            {
                return null;
            }

            return new GetFreelancerDto
            {
                Email = user.Email,
                PublicProfileDescription = user.PublicProfileDescription,
                ContactNumber = user.ContactNumber,
                LegalResponsibleDocument = user.LegalResponsibleDocument,
                LegalResponsibleFullName = user.LegalResponsibleFullName,
                Address = user.Address,
                AddressNumber = user.AddressNumber,
                Quarter = user.Quarter,
                City = user.City,
                State = user.State,
                AdditionalAddressInfo = user.AdditionalAddressInfo,
                PostalCode = user.PostalCode,
                HasFixedProducer = fields.HasFixedProducer,
                HasOwnCar = fields.HasOwnCar,
                BirthDate = fields.BirthDate,
                OwnMachineNames = fields.OwnMachines,
                SpecialtyNames = fields.Specialties,
                AvailableTimeName = fields.AvailableTimeName,
                AverageRevenueName = fields.AverageRevenueName,
                HowUsuallyArrangeServicesName = fields.HowUsuallyArrangeServicesName,
                BusinessTypeName = fields.BusinessTypeName,
                ExperienceYearsName = fields.ExperienceYearsName,
                FreelancerPreferencesName = fields.FreelancerPreferencesName,
                WorkshopSizeName = fields.WorkshopSizeName
            };
        }

        public async Task<GetCompanyDto?> GetCompanyProfileByEmailAsync(string userEmail)
        {
            User? user = await _userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return null;
            }

            CompanyProfile? profile = await _userRepository.GetCompanyProfileAsync(user.Id);
            if (profile == null)
            {
                return null;
            }

            return new GetCompanyDto
            {
                Email = user.Email,
                PublicProfileDescription = user.PublicProfileDescription,
                ContactNumber = user.ContactNumber,
                LegalResponsibleDocument = user.LegalResponsibleDocument,
                LegalResponsibleFullName = user.LegalResponsibleFullName,
                Address = user.Address,
                AddressNumber = user.AddressNumber,
                Quarter = user.Quarter,
                City = user.City,
                State = user.State,
                AdditionalAddressInfo = user.AdditionalAddressInfo,
                PostalCode = user.PostalCode,
                LegalName = profile.LegalName,
                CompanyName = profile.CompanyName,
                CompanyRegistrationDocument = profile.CompanyRegistrationDocument,
                CoreBusiness = profile.CoreBusiness
            };
        }

        public async Task<ICollection<int>> GetUserRolesAsync(int id)
        {
            ICollection<UserRole> allUserRoles = await _userRepository.GetUserRolesAsync(id);

            List<int> rolesInteger = [];

            foreach (UserRole role in allUserRoles)
            {
                rolesInteger.Add(role.RoleId);
            }

            return rolesInteger;
        }



        // Atualização de perfil
        public async Task<ProfileUpdateResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto, string userEmail)
        {
            User? user = await _userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            FreelancerProfile? profile = await _userRepository.GetFreelancerProfileAsync(user.Id);
            if (profile == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            if (dto.Email != null && dto.Email != user.Email &&
                await _userRepository.IsUserEmailRegistered(dto.Email))
            {
                return ProfileUpdateResult.EmailInUse;
            }

            ApplyUserChanges(user, dto);

            if (dto.BirthDate != null && dto.BirthDate != profile.BirthDate)
            {
                profile.BirthDate = (DateTime)dto.BirthDate;
            }


            if (dto.BusinessTypeId != null && dto.BusinessTypeId != profile.BusinessTypeId)
            {
                if (await _freelancerFieldsRepository.BusinessTypeExistsAsync((int)dto.BusinessTypeId))
                {
                    profile.BusinessTypeId = (int)dto.BusinessTypeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.ExperienceYearsId != null && dto.ExperienceYearsId != profile.ExperienceYearsId)
            {
                if (await _freelancerFieldsRepository.ExperienceYearsExistsAsync((int)dto.ExperienceYearsId))
                {
                    profile.ExperienceYearsId = (int)dto.ExperienceYearsId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.WorkshopSizeId != null && dto.WorkshopSizeId != profile.WorkshopSizeId)
            {
                if (await _freelancerFieldsRepository.WorkshopSizeExistsAsync((int)dto.WorkshopSizeId))
                {
                    profile.WorkshopSizeId = (int)dto.WorkshopSizeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.HowUsuallyArrangeServicesId != null && dto.HowUsuallyArrangeServicesId != profile.HowUsuallyArrangeServicesId)
            {
                if (await _freelancerFieldsRepository.HowUsuallyArrangeServicesExistsAsync((int)dto.HowUsuallyArrangeServicesId))
                {
                    profile.HowUsuallyArrangeServicesId = (int)dto.HowUsuallyArrangeServicesId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.AvailableTimeId != null && dto.AvailableTimeId != profile.AvailableTimeId)
            {
                if (await _freelancerFieldsRepository.AvailableTimeExistsAsync((int)dto.AvailableTimeId))
                {
                    profile.AvailableTimeId = (int)dto.AvailableTimeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.FreelancerPreferencesId != null && dto.FreelancerPreferencesId != profile.FreelancerPreferencesId)
            {
                if (await _freelancerFieldsRepository.FreelancerPreferencesExistsAsync((int)dto.FreelancerPreferencesId))
                {
                    profile.FreelancerPreferencesId = (int)dto.FreelancerPreferencesId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.AverageRevenueId != null && dto.AverageRevenueId != profile.AverageRevenueId)
            {
                if (await _freelancerFieldsRepository.AverageRevenueExistsAsync((int)dto.AverageRevenueId))
                {
                    profile.AverageRevenueId = (int)dto.AverageRevenueId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.HasFixedProducer != null && dto.HasFixedProducer != profile.HasFixedProducer)
            {
                profile.HasFixedProducer = (bool)dto.HasFixedProducer;
            }


            if (dto.HasOwnCar != null && dto.HasOwnCar != profile.HasOwnCar)
            {
                profile.HasOwnCar = (bool)dto.HasOwnCar;
            }

            // SpecialtyIds
            List<FreelancerSpecialties> newFreelancerSpecialtiesList = new();

            if (dto.SpecialtyIds != null)
            {
                if (await _freelancerFieldsRepository.SpecialtyExistsAsync(dto.SpecialtyIds))
                {
                    foreach (int specialtyId in dto.SpecialtyIds)
                    {
                        FreelancerSpecialties newFreelaSpecialty = new()
                        {
                            SpecialtyId = specialtyId,
                            FreelancerId = profile.Id
                        };

                        newFreelancerSpecialtiesList.Add(newFreelaSpecialty);
                    }
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            // OwnMachineIds
            List<FreelancerOwnMachines> newFreelancerOwnMachinesList = new();

            if (dto.OwnMachineIds != null)
            {
                if (await _freelancerFieldsRepository.OwnMachinesExistsAsync(dto.OwnMachineIds))
                {
                    foreach (int ownMachineId in dto.OwnMachineIds)
                    {
                        FreelancerOwnMachines newfreelancerOwnMachines = new()
                        {
                            OwnMachineId = ownMachineId,
                            FreelancerId = profile.Id
                        };

                        newFreelancerOwnMachinesList.Add(newfreelancerOwnMachines);
                    }
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            await _userRepository.UpdateFreelancerAsync(profile.Id, newFreelancerSpecialtiesList, newFreelancerOwnMachinesList);

            return ProfileUpdateResult.Success;
        }

        public async Task<ProfileUpdateResult> UpdateUserCompanyAsync(UpdateCompanyDto dto, string userEmail)
        {
            User? user = await _userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            CompanyProfile? profile = await _userRepository.GetCompanyProfileAsync(user.Id);
            if (profile == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            if (dto.Email != null && dto.Email != user.Email &&
                await _userRepository.IsUserEmailRegistered(dto.Email))
            {
                return ProfileUpdateResult.EmailInUse;
            }

            ApplyUserChanges(user, dto);

            if (dto.LegalName != null && dto.LegalName != profile.LegalName)
            {
                profile.LegalName = dto.LegalName;
            }

            if (dto.CompanyName != null && dto.CompanyName != profile.CompanyName)
            {
                profile.CompanyName = dto.CompanyName;
            }

            if (dto.CompanyRegistrationDocument != null && dto.CompanyRegistrationDocument != profile.CompanyRegistrationDocument)
            {
                profile.CompanyRegistrationDocument = dto.CompanyRegistrationDocument;
            }

            if (dto.CoreBusiness != null && dto.CoreBusiness != profile.CoreBusiness)
            {
                profile.CoreBusiness = dto.CoreBusiness;
            }

            await _userRepository.UpdateCompanyAsync();
            return ProfileUpdateResult.Success;
        }

        // Excluir perfil

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            await _userRepository.RemoveRoleFromUserAsync(userId, roleId);
        }

        public async Task RemoveAllRolesFromUserAsync(int id)
        {
            await _userRepository.RemoveAllRolesFromUserAsync(id);
        }

        public async Task<bool> DeleteCurrentUserAsync(string userEmail)
        {
            User? user = await _userRepository.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteCurrentUserAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserByIdAsync(int id)
        {
            User? user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteCurrentUserAsync(user);
            return true;
        }

        // Verificação

        public async Task<bool> IsUserEmailRegistered(string email)
        {
            return await _userRepository.IsUserEmailRegistered(email);
        }

        public async Task<PasswordVerificationResult> VerifyPassword(int userId, string password)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return PasswordVerificationResult.Failed;
            }

            return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        }

        private static void ApplyUserChanges(User user, UserDto dto)
        {
            if (dto.LegalResponsibleFullName != null && dto.LegalResponsibleFullName != user.LegalResponsibleFullName)
            {
                user.LegalResponsibleFullName = dto.LegalResponsibleFullName;
            }

            if (dto.LegalResponsibleDocument != null && dto.LegalResponsibleDocument != user.LegalResponsibleDocument)
            {
                user.LegalResponsibleDocument = dto.LegalResponsibleDocument;
            }

            if (dto.Email != null && dto.Email != user.Email)
            {
                user.Email = dto.Email;
            }

            if (dto.ContactNumber != null && dto.ContactNumber != user.ContactNumber)
            {
                user.ContactNumber = dto.ContactNumber;
            }

            if (dto.PostalCode != null && dto.PostalCode != user.PostalCode)
            {
                user.PostalCode = dto.PostalCode;
            }

            if (dto.Address != null && dto.Address != user.Address)
            {
                user.Address = dto.Address;
            }

            if (dto.AddressNumber != null && dto.AddressNumber != user.AddressNumber)
            {
                user.AddressNumber = (int)dto.AddressNumber;
            }

            if (dto.Quarter != null && dto.Quarter != user.Quarter)
            {
                user.Quarter = dto.Quarter;
            }

            if (dto.AdditionalAddressInfo != null && dto.AdditionalAddressInfo != user.AdditionalAddressInfo)
            {
                user.AdditionalAddressInfo = dto.AdditionalAddressInfo;
            }

            if (dto.City != null && dto.City != user.City)
            {
                user.City = dto.City;
            }

            if (dto.State != null && dto.State != user.State)
            {
                user.State = dto.State;
            }

            if (dto.PublicProfileDescription != null && dto.PublicProfileDescription != user.PublicProfileDescription)
            {
                user.PublicProfileDescription = dto.PublicProfileDescription;
            }
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                LegalResponsibleFullName = user.LegalResponsibleFullName,
                LegalResponsibleDocument = user.LegalResponsibleDocument,
                ContactNumber = user.ContactNumber,
                Email = user.Email,
                PublicProfileDescription = user.PublicProfileDescription,
                Address = user.Address,
                AddressNumber = user.AddressNumber,
                Quarter = user.Quarter,
                PostalCode = user.PostalCode,
                City = user.City,
                State = user.State,
                AdditionalAddressInfo = user.AdditionalAddressInfo
            };
        }
    }
}