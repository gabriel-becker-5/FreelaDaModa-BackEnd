using _02_Application.DTOs;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _02_Application.Validation;
using _04_Domain.Entities.Identity;
using _04_Domain.Entities.Profiles;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        // Leitura de usuário
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            User? result = await _userRepository.GetUserByIdAsync(id);

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

        public async Task<AuthenticatedUserDto?> AuthenticateAsync(string email, string password)
        {
            User? user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            if (user.Roles.Count == 0)
            {
                return null;
            }

            return new AuthenticatedUserDto
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = user.Roles.Select(role => role.ToString().Replace("_", " ")).ToList()
            };
        }

        public async Task<PagedResult<UserDto>> GetAllUsersAsync(int page, int pageSize)
        {
            page = Math.Clamp(page, 1, 1000);
            pageSize = Math.Clamp(pageSize, 1, 50);

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
        public async Task<GetFreelancerDto?> GetFreelancerProfileByIdAsync(int userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            FreelancerProfile? profile = await _userRepository.GetFreelancerProfileAsync(user.Id);
            if (profile == null)
            {
                return null;
            }

            return MapToFreelancerDto(user, profile);
        }

        public async Task<GetCompanyDto?> GetCompanyProfileByIdAsync(int userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            CompanyProfile? profile = await _userRepository.GetCompanyProfileAsync(user.Id);
            if (profile == null)
            {
                return null;
            }

            return MapToCompanyDto(user, profile);
        }

        // Atualização de perfil
        public async Task<ProfileUpdateResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto, int userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            FreelancerProfile? profile = await _userRepository.GetFreelancerProfileAsync(user.Id);
            if (profile == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            if (!string.IsNullOrEmpty(dto.Email) 
                && dto.Email.ToLower() != user.Email.ToLower()
                && await _userRepository.IsEmailRegistered(dto.Email))
            {
                return ProfileUpdateResult.EmailInUse;
            }

            if (!string.IsNullOrEmpty(dto.LegalResponsibleDocument)
                && dto.LegalResponsibleDocument.ToLower() != user.LegalResponsibleDocument.ToLower()
                && await _userRepository.IsCpfRegistered(dto.LegalResponsibleDocument))
            {
                return ProfileUpdateResult.DocumentInUse;
            }

            ApplyUserChanges(user, dto);

            if (dto.BirthDate.HasValue 
                    && dto.BirthDate != profile.BirthDate) // se tem valor e é diferente do atual, segue.
            {
                if (dto.BirthDate.Value != default(DateTime) 
                        && dto.BirthDate.Value < DateTime.UtcNow) // se dto não é padrão e dto é menor igual agora
                {
                    profile.BirthDate = (DateTime)dto.BirthDate;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.BusinessTypeId.HasValue 
                    && (BusinessType)dto.BusinessTypeId != profile.BusinessTypeId
                    && dto.BusinessTypeId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<BusinessType>((int)dto.BusinessTypeId))
                {
                    profile.BusinessTypeId = (BusinessType)dto.BusinessTypeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.ExperienceYearsId.HasValue 
                    && (ExperienceYears)dto.ExperienceYearsId != profile.ExperienceYearsId
                    && dto.ExperienceYearsId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<ExperienceYears>((int)dto.ExperienceYearsId))
                {
                    profile.ExperienceYearsId = (ExperienceYears)dto.ExperienceYearsId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.WorkshopSizeId.HasValue 
                    && (WorkshopSize)dto.WorkshopSizeId != profile.WorkshopSizeId
                    && dto.WorkshopSizeId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<WorkshopSize>((int)dto.WorkshopSizeId))
                {
                    profile.WorkshopSizeId = (WorkshopSize)dto.WorkshopSizeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.HowUsuallyArrangeServicesId.HasValue 
                    && (HowUsuallyArrangeServices)dto.HowUsuallyArrangeServicesId != profile.HowUsuallyArrangeServicesId
                    && dto.HowUsuallyArrangeServicesId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<HowUsuallyArrangeServices>((int)dto.HowUsuallyArrangeServicesId))
                {
                    profile.HowUsuallyArrangeServicesId = (HowUsuallyArrangeServices)dto.HowUsuallyArrangeServicesId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.AvailableTimeId.HasValue 
                    && (AvailableTime)dto.AvailableTimeId != profile.AvailableTimeId
                    && dto.AvailableTimeId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<AvailableTime>((int)dto.AvailableTimeId))
                {
                    profile.AvailableTimeId = (AvailableTime)dto.AvailableTimeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.FreelancerPreferencesId.HasValue 
                    && (FreelancerPreferences)dto.FreelancerPreferencesId != profile.FreelancerPreferencesId
                    && dto.FreelancerPreferencesId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<FreelancerPreferences>((int)dto.FreelancerPreferencesId))
                {
                    profile.FreelancerPreferencesId = (FreelancerPreferences)dto.FreelancerPreferencesId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.AverageRevenueId.HasValue 
                    && (AverageRevenue)dto.AverageRevenueId != profile.AverageRevenueId
                    && dto.AverageRevenueId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<AverageRevenue>((int)dto.AverageRevenueId))
                {
                    profile.AverageRevenueId = (AverageRevenue)dto.AverageRevenueId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.HasFixedProducer.HasValue && dto.HasFixedProducer != profile.HasFixedProducer)
            {
                profile.HasFixedProducer = (bool)dto.HasFixedProducer;
            }

            if (dto.HasOwnCar.HasValue && dto.HasOwnCar != profile.HasOwnCar)
            {
                profile.HasOwnCar = (bool)dto.HasOwnCar;
            }

            // SpecialtyIds
            if (dto.SpecialtyIds != null && dto.SpecialtyIds.Any())
            {
                if (!dto.SpecialtyIds.Any(id => id == 0))
                {
                    if (dto.SpecialtyIds.Any(id => id == null))
                    {
                        return ProfileUpdateResult.InvalidData;
                    }

                    if (!FreelancerFieldValidator.IsIdValid<Specialty>(dto.SpecialtyIds.Cast<int>().ToList()))
                    {
                        return ProfileUpdateResult.InvalidData;
                    }

                    profile.SpecialtiesIds.Clear();
                    foreach (int specialtyId in dto.SpecialtyIds)
                    {
                        profile.SpecialtiesIds.Add((Specialty)specialtyId);
                    }
                }
            }

            // OwnMachineIds
            if (dto.OwnMachineIds != null && dto.OwnMachineIds.Any())
            {
                if (!dto.OwnMachineIds.Any(id => id == 0))
                {
                    if (dto.OwnMachineIds.Any(id => id == null))
                    {
                        return ProfileUpdateResult.InvalidData;
                    }

                    if (!FreelancerFieldValidator.IsIdValid<OwnMachine>(dto.OwnMachineIds.Cast<int>().ToList()))
                    {
                        return ProfileUpdateResult.InvalidData;
                    }

                    profile.OwnMachinesIds.Clear();
                    foreach (int machineId in dto.OwnMachineIds)
                    {
                        profile.OwnMachinesIds.Add((OwnMachine)machineId);
                    }
                }
            }

            await _userRepository.UpdateFreelancerAsync(user, profile);
            return ProfileUpdateResult.Success;
        }

        public async Task<ProfileUpdateResult> UpdateUserCompanyAsync(UpdateCompanyDto dto, int userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            CompanyProfile? profile = await _userRepository.GetCompanyProfileAsync(user.Id);
            if (profile == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            if (!string.IsNullOrEmpty(dto.Email)
                && dto.Email.ToLower() != user.Email.ToLower()
                && await _userRepository.IsEmailRegistered(dto.Email))
            {
                return ProfileUpdateResult.EmailInUse;
            }

            if (!string.IsNullOrEmpty(dto.CompanyRegistrationDocument)
                && dto.CompanyRegistrationDocument.ToLower() != profile.CompanyRegistrationDocument.ToLower()
                && await _userRepository.IsCnpjRegistered(dto.CompanyRegistrationDocument))
            {
                return ProfileUpdateResult.DocumentInUse;
            }

            if (!string.IsNullOrEmpty(dto.CompanyRegistrationDocument)
                    && dto.CompanyRegistrationDocument.ToLower() != profile.CompanyRegistrationDocument.ToLower())
            {
                profile.CompanyRegistrationDocument = dto.CompanyRegistrationDocument;
            }

            ApplyUserChanges(user, dto);

            if (!string.IsNullOrEmpty(dto.LegalName)
                && dto.LegalName.ToLower() != profile.LegalName.ToLower())
            {
                profile.LegalName = dto.LegalName;
            }

            if (!string.IsNullOrEmpty(dto.CompanyName) 
                && dto.CompanyName.ToLower() != profile.CompanyName.ToLower())
            {
                profile.CompanyName = dto.CompanyName;
            }

            if (!string.IsNullOrEmpty(dto.CoreBusiness)
                && dto.CoreBusiness.ToLower() != profile.CoreBusiness.ToLower())
            {
                profile.CoreBusiness = dto.CoreBusiness;
            }

            await _userRepository.UpdateCompanyAsync(user, profile);
            return ProfileUpdateResult.Success;
        }

        // Excluir perfil
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

        // Helpers de Enums e Validações
        private static void ApplyUserChanges(User user, UpdateUserDto dto)
        {
            if (!string.IsNullOrEmpty(dto.LegalResponsibleFullName) 
                && dto.LegalResponsibleFullName.ToLower() != user.LegalResponsibleFullName.ToLower())
            {
                user.LegalResponsibleFullName = dto.LegalResponsibleFullName;
            }

            if (!string.IsNullOrEmpty(dto.LegalResponsibleDocument) 
                && dto.LegalResponsibleDocument.ToLower() != user.LegalResponsibleDocument.ToLower())
            {
                user.LegalResponsibleDocument = dto.LegalResponsibleDocument;
            }

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email.ToLower() != user.Email.ToLower())
            {
                user.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.ContactNumber) 
                && dto.ContactNumber.ToLower() != user.ContactNumber.ToLower())
            {
                user.ContactNumber = dto.ContactNumber;
            }

            if (!string.IsNullOrEmpty(dto.PostalCode) 
                && dto.PostalCode.ToLower() != user.PostalCode.ToLower())
            {
                user.PostalCode = dto.PostalCode;
            }

            if (!string.IsNullOrEmpty(dto.Address) 
                && dto.Address.ToLower() != user.Address.ToLower())
            {
                user.Address = dto.Address;
            }

            if (dto.AddressNumber.HasValue 
                    && dto.AddressNumber != user.AddressNumber
                    && dto.AddressNumber.Value != 0)
            {
                user.AddressNumber = (int)dto.AddressNumber;
            }

            if (!string.IsNullOrEmpty(dto.Neighborhood) 
                && dto.Neighborhood.ToLower() != user.Neighborhood.ToLower())
            {
                user.Neighborhood = dto.Neighborhood;
            }

            if (dto.AdditionalAddressInfo == "")
            {
                user.AdditionalAddressInfo = null; // permite limpar o complemento
            }
            else if (dto.AdditionalAddressInfo != null 
                && (user.AdditionalAddressInfo == null
                    || dto.AdditionalAddressInfo.ToLower() != user.AdditionalAddressInfo.ToLower()))
            {
                user.AdditionalAddressInfo = dto.AdditionalAddressInfo;
            }

            if (!string.IsNullOrEmpty(dto.City) 
                && dto.City.ToLower() != user.City.ToLower())
            {
                user.City = dto.City;
            }

            if (!string.IsNullOrEmpty(dto.State) 
                && dto.State.ToLower() != user.State.ToLower())
            {
                user.State = dto.State;
            }

            if (!string.IsNullOrEmpty(dto.PublicProfileDescription) 
                && dto.PublicProfileDescription.ToLower() != user.PublicProfileDescription.ToLower())
            {
                user.PublicProfileDescription = dto.PublicProfileDescription;
            }
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                LegalResponsibleFullName = user.LegalResponsibleFullName,
                ContactNumber = user.ContactNumber,
                Email = user.Email,
                PublicProfileDescription = user.PublicProfileDescription,
                Address = user.Address,
                AddressNumber = user.AddressNumber,
                Neighborhood = user.Neighborhood,
                PostalCode = user.PostalCode,
                City = user.City,
                State = user.State,
                AdditionalAddressInfo = user.AdditionalAddressInfo
            };
        }

        private static GetFreelancerDto MapToFreelancerDto(User user, FreelancerProfile profile)
        {
            return new GetFreelancerDto
            {
                Email = user.Email,
                PublicProfileDescription = user.PublicProfileDescription,
                ContactNumber = user.ContactNumber,
                LegalResponsibleFullName = user.LegalResponsibleFullName,
                Address = user.Address,
                AddressNumber = user.AddressNumber,
                Neighborhood = user.Neighborhood,
                City = user.City,
                State = user.State,
                AdditionalAddressInfo = user.AdditionalAddressInfo,
                PostalCode = user.PostalCode,
                HasFixedProducer = profile.HasFixedProducer,
                HasOwnCar = profile.HasOwnCar,
                BirthDate = profile.BirthDate,
                AvailableTimeName = profile.AvailableTimeId.ToString().Replace("_", " "),
                AverageRevenueName = profile.AverageRevenueId.ToString().Replace("_", " "),
                HowUsuallyArrangeServicesName = profile.HowUsuallyArrangeServicesId.ToString().Replace("_", " "),
                BusinessTypeName = profile.BusinessTypeId.ToString().Replace("_", " "),
                ExperienceYearsName = profile.ExperienceYearsId.ToString().Replace("_", " "),
                FreelancerPreferencesName = profile.FreelancerPreferencesId.ToString().Replace("_", " "),
                WorkshopSizeName = profile.WorkshopSizeId.ToString().Replace("_", " "),
                OwnMachineNames = profile.OwnMachinesIds
                                    .Select(id => id
                                    .ToString()
                                    .Replace("_", " "))
                                    .ToList(),
                SpecialtyNames = profile.SpecialtiesIds
                                    .Select(id => id
                                    .ToString()
                                    .Replace("_", " "))
                                    .ToList()
            };
        }

        private static GetCompanyDto MapToCompanyDto(User user, CompanyProfile profile)
        {
            return new GetCompanyDto
            {
                Email = user.Email,
                PublicProfileDescription = user.PublicProfileDescription,
                ContactNumber = user.ContactNumber,
                LegalResponsibleFullName = user.LegalResponsibleFullName,
                Address = user.Address,
                AddressNumber = user.AddressNumber,
                Neighborhood = user.Neighborhood,
                City = user.City,
                State = user.State,
                AdditionalAddressInfo = user.AdditionalAddressInfo,
                PostalCode = user.PostalCode,
                LegalName = profile.LegalName,
                CompanyName = profile.CompanyName,
                CoreBusiness = profile.CoreBusiness
            };
        }

        public async Task<int?> CreateFreelancerAsync(CreateFreelancerDto dto)
        {
            if (await _userRepository.IsEmailRegistered(dto.Email))
            {
                return null;
            }

            if (await _userRepository.IsCpfRegistered(dto.LegalResponsibleDocument))
            {
                return null;
            }

            if (FreelancerFieldValidator.IsFreelancerFieldsValid(dto).Count > 0)
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
                Neighborhood = dto.Neighborhood,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                AdditionalAddressInfo = dto.AdditionalAddressInfo,
                Roles = new List<Roles> { Roles.Freelancer }
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(dto.Password);

            FreelancerProfile newFreelancer = new()
            {
                BirthDate = dto.BirthDate,
                HasFixedProducer = dto.HasFixedProducer,
                HasOwnCar = dto.HasOwnCar,
                AvailableTimeId = (AvailableTime)dto.AvailableTimeId,
                AverageRevenueId = (AverageRevenue)dto.AverageRevenueId,
                BusinessTypeId = (BusinessType)dto.BusinessTypeId,
                ExperienceYearsId = (ExperienceYears)dto.ExperienceYearsId,
                FreelancerPreferencesId = (FreelancerPreferences)dto.FreelancerPreferencesId,
                HowUsuallyArrangeServicesId = (HowUsuallyArrangeServices)dto.HowUsuallyArrangeServicesId,
                WorkshopSizeId = (WorkshopSize)dto.WorkshopSizeId,
                SpecialtiesIds = dto.SpecialtyIds.Select(id => (Specialty)id).ToList(),
                OwnMachinesIds = dto.OwnMachineIds.Select(id => (OwnMachine)id).ToList()
            };

            return await _userRepository.CreateFreelancerUserProfileAsync(newUser, newFreelancer);
        }

        public async Task<int?> CreateCompanyAsync(CreateCompanyDto dto)
        {
            if (await _userRepository.IsEmailRegistered(dto.Email))
            {
                return null;
            }

            if (await _userRepository.IsCpfRegistered(dto.LegalResponsibleDocument))
            {
                return null;
            }

            if (await _userRepository.IsCnpjRegistered(dto.CompanyRegistrationDocument))
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
                Neighborhood = dto.Neighborhood,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                AdditionalAddressInfo = dto.AdditionalAddressInfo,
                Roles = new List<Roles> { Roles.Company }
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(dto.Password);

            CompanyProfile newCompany = new()
            {
                CompanyName = dto.CompanyName,
                CoreBusiness = dto.CoreBusiness,
                CompanyRegistrationDocument = dto.CompanyRegistrationDocument,
                LegalName = dto.LegalName
            };

            return await _userRepository.CreateCompanyUserProfileAsync(newUser, newCompany);
        }
    }
}