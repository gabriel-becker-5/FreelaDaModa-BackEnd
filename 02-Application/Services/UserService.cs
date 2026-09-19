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
            User? user = await _userRepository.GetFreelancerProfileAsync(userId);

            if (user == null)
            {
                return null;
            }

            return MapToFreelancerDto(user);
        }

        public async Task<GetCompanyDto?> GetCompanyProfileByIdAsync(int userId)
        {
            User? user = await _userRepository.GetCompanyProfileAsync(userId);
            if (user == null)
            {
                return null;
            }

            return MapToCompanyDto(user);
        }

        // Atualização de perfil
        public async Task<ProfileUpdateResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto, int userId)
        {
            User? user = await _userRepository.GetFreelancerProfileAsync(userId);
            if (user == null)
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
                    && dto.BirthDate != user.FreelancerProfile.BirthDate) // se tem valor e é diferente do atual, segue.
            {
                if (dto.BirthDate.Value != default(DateTime)
                        && dto.BirthDate.Value < DateTime.UtcNow) // se dto não é padrão e dto é menor igual agora
                {
                    user.FreelancerProfile.BirthDate = (DateTime)dto.BirthDate;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.BusinessTypeId.HasValue
                    && (BusinessType)dto.BusinessTypeId != user.FreelancerProfile.BusinessTypeId
                    && dto.BusinessTypeId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<BusinessType>((int)dto.BusinessTypeId))
                {
                    user.FreelancerProfile.BusinessTypeId = (BusinessType)dto.BusinessTypeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.ExperienceYearsId.HasValue
                    && (ExperienceYears)dto.ExperienceYearsId != user.FreelancerProfile.ExperienceYearsId
                    && dto.ExperienceYearsId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<ExperienceYears>((int)dto.ExperienceYearsId))
                {
                    user.FreelancerProfile.ExperienceYearsId = (ExperienceYears)dto.ExperienceYearsId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.WorkshopSizeId.HasValue
                    && (WorkshopSize)dto.WorkshopSizeId != user.FreelancerProfile.WorkshopSizeId
                    && dto.WorkshopSizeId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<WorkshopSize>((int)dto.WorkshopSizeId))
                {
                    user.FreelancerProfile.WorkshopSizeId = (WorkshopSize)dto.WorkshopSizeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.HowUsuallyArrangeServicesId.HasValue
                    && (HowUsuallyArrangeServices)dto.HowUsuallyArrangeServicesId != user.FreelancerProfile.HowUsuallyArrangeServicesId
                    && dto.HowUsuallyArrangeServicesId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<HowUsuallyArrangeServices>((int)dto.HowUsuallyArrangeServicesId))
                {
                    user.FreelancerProfile.HowUsuallyArrangeServicesId = (HowUsuallyArrangeServices)dto.HowUsuallyArrangeServicesId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.AvailableTimeId.HasValue
                    && (AvailableTime)dto.AvailableTimeId != user.FreelancerProfile.AvailableTimeId
                    && dto.AvailableTimeId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<AvailableTime>((int)dto.AvailableTimeId))
                {
                    user.FreelancerProfile.AvailableTimeId = (AvailableTime)dto.AvailableTimeId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.FreelancerPreferencesId.HasValue
                    && (FreelancerPreferences)dto.FreelancerPreferencesId != user.FreelancerProfile.FreelancerPreferencesId
                    && dto.FreelancerPreferencesId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<FreelancerPreferences>((int)dto.FreelancerPreferencesId))
                {
                    user.FreelancerProfile.FreelancerPreferencesId = (FreelancerPreferences)dto.FreelancerPreferencesId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.AverageRevenueId.HasValue
                    && (AverageRevenue)dto.AverageRevenueId != user.FreelancerProfile.AverageRevenueId
                    && dto.AverageRevenueId != 0)
            {
                if (FreelancerFieldValidator.IsIdValid<AverageRevenue>((int)dto.AverageRevenueId))
                {
                    user.FreelancerProfile.AverageRevenueId = (AverageRevenue)dto.AverageRevenueId;
                }
                else
                {
                    return ProfileUpdateResult.InvalidData;
                }
            }

            if (dto.HasFixedProducer.HasValue && dto.HasFixedProducer != user.FreelancerProfile.HasFixedProducer)
            {
                user.FreelancerProfile.HasFixedProducer = (bool)dto.HasFixedProducer;
            }

            if (dto.HasOwnCar.HasValue && dto.HasOwnCar != user.FreelancerProfile.HasOwnCar)
            {
                user.FreelancerProfile.HasOwnCar = (bool)dto.HasOwnCar;
            }

            // SpecialtyIds
            if (dto.SpecialtyIds != null && dto.SpecialtyIds.Any())
            {
                if (!dto.SpecialtyIds.Any(id => id == 0))
                {
                    if (!FreelancerFieldValidator.IsIdValid<Specialty>(dto.SpecialtyIds.Cast<int>().ToList()))
                    {
                        return ProfileUpdateResult.InvalidData;
                    }

                    user.FreelancerProfile.SpecialtiesIds.Clear();
                    foreach (int specialtyId in dto.SpecialtyIds)
                    {
                        user.FreelancerProfile.SpecialtiesIds.Add((Specialty)specialtyId);
                    }
                }
            }

            // OwnMachineIds
            if (dto.OwnMachineIds != null && dto.OwnMachineIds.Any())
            {
                if (!dto.OwnMachineIds.Any(id => id == 0))
                {
                    if (!FreelancerFieldValidator.IsIdValid<OwnMachine>(dto.OwnMachineIds.Cast<int>().ToList()))
                    {
                        return ProfileUpdateResult.InvalidData;
                    }

                    user.FreelancerProfile.OwnMachinesIds.Clear();
                    foreach (int machineId in dto.OwnMachineIds)
                    {
                        user.FreelancerProfile.OwnMachinesIds.Add((OwnMachine)machineId);
                    }
                }
            }

            await _userRepository.UpdateFreelancerAsync(user, user.FreelancerProfile);
            return ProfileUpdateResult.Success;
        }

        public async Task<ProfileUpdateResult> UpdateUserCompanyAsync(UpdateCompanyDto dto, int userId)
        {
            User? user = await _userRepository.GetCompanyProfileAsync(userId);
            if (user == null)
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
                && dto.CompanyRegistrationDocument.ToLower() != user.CompanyProfile.CompanyRegistrationDocument.ToLower()
                && await _userRepository.IsCnpjRegistered(dto.CompanyRegistrationDocument))
            {
                return ProfileUpdateResult.DocumentInUse;
            }

            if (!string.IsNullOrEmpty(dto.CompanyRegistrationDocument)
                    && dto.CompanyRegistrationDocument.ToLower() != user.CompanyProfile.CompanyRegistrationDocument.ToLower())
            {
                user.CompanyProfile.CompanyRegistrationDocument = dto.CompanyRegistrationDocument;
            }

            ApplyUserChanges(user, dto);

            if (!string.IsNullOrEmpty(dto.LegalName)
                && dto.LegalName.ToLower() != user.CompanyProfile.LegalName.ToLower())
            {
                user.CompanyProfile.LegalName = dto.LegalName;
            }

            if (!string.IsNullOrEmpty(dto.CompanyName)
                && dto.CompanyName.ToLower() != user.CompanyProfile.CompanyName.ToLower())
            {
                user.CompanyProfile.CompanyName = dto.CompanyName;
            }

            if (!string.IsNullOrEmpty(dto.CoreBusiness)
                && dto.CoreBusiness.ToLower() != user.CompanyProfile.CoreBusiness.ToLower())
            {
                user.CompanyProfile.CoreBusiness = dto.CoreBusiness;
            }

            await _userRepository.UpdateCompanyAsync(user, user.CompanyProfile);
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
                && (dto.PublicProfileDescription == null
                    || dto.PublicProfileDescription.ToLower() != user.PublicProfileDescription.ToLower()))
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

        private static GetFreelancerDto MapToFreelancerDto(User user)
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
                HasFixedProducer = user.FreelancerProfile.HasFixedProducer,
                HasOwnCar = user.FreelancerProfile.HasOwnCar,
                BirthDate = user.FreelancerProfile.BirthDate,
                AvailableTimeName = user.FreelancerProfile.AvailableTimeId.ToString().Replace("_", " "),
                AverageRevenueName = user.FreelancerProfile.AverageRevenueId.ToString().Replace("_", " "),
                HowUsuallyArrangeServicesName = user.FreelancerProfile.HowUsuallyArrangeServicesId.ToString().Replace("_", " "),
                BusinessTypeName = user.FreelancerProfile.BusinessTypeId.ToString().Replace("_", " "),
                ExperienceYearsName = user.FreelancerProfile.ExperienceYearsId.ToString().Replace("_", " "),
                FreelancerPreferencesName = user.FreelancerProfile.FreelancerPreferencesId.ToString().Replace("_", " "),
                WorkshopSizeName = user.FreelancerProfile.WorkshopSizeId.ToString().Replace("_", " "),
                OwnMachineNames = user.FreelancerProfile.OwnMachinesIds
                                    .Select(id => id
                                    .ToString()
                                    .Replace("_", " "))
                                    .ToList(),
                SpecialtyNames = user.FreelancerProfile.SpecialtiesIds
                                    .Select(id => id
                                    .ToString()
                                    .Replace("_", " "))
                                    .ToList()
            };
        }

        private static GetCompanyDto MapToCompanyDto(User user)
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
                LegalName = user.CompanyProfile.LegalName,
                CompanyName = user.CompanyProfile.CompanyName,
                CoreBusiness = user.CompanyProfile.CoreBusiness
            };
        }

        public async Task<CreateUserResult> CreateFreelancerAsync(CreateFreelancerDto dto)
        {
            if (await _userRepository.IsEmailRegistered(dto.Email))
            {
                return new CreateUserResult { Status = CreateUserStatus.EmailInUse };
            }

            if (await _userRepository.IsCpfRegistered(dto.LegalResponsibleDocument))
            {
                return new CreateUserResult { Status = CreateUserStatus.CpfInUse };
            }

            ICollection<EnumValidationError> errors =
                FreelancerFieldValidator.IsFreelancerFieldsValid(dto);

            if (errors.Count > 0)
            {
                return new CreateUserResult { Status = CreateUserStatus.InvalidData, Errors = errors };
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

            int? userId = await _userRepository.CreateFreelancerUserProfileAsync(newUser, newFreelancer);

            if (userId == null)
            {
                return new CreateUserResult { Status = CreateUserStatus.DatabaseError };
            }

            return new CreateUserResult { Status = CreateUserStatus.Success, UserId = userId };
        }

        public async Task<CreateUserResult> CreateCompanyAsync(CreateCompanyDto dto)
        {
            if (await _userRepository.IsEmailRegistered(dto.Email))
            {
                return new CreateUserResult { Status = CreateUserStatus.EmailInUse };
            }

            if (await _userRepository.IsCpfRegistered(dto.LegalResponsibleDocument))
            {
                return new CreateUserResult { Status = CreateUserStatus.CpfInUse };
            }

            if (await _userRepository.IsCnpjRegistered(dto.CompanyRegistrationDocument))
            {
                return new CreateUserResult { Status = CreateUserStatus.CnpjInUse };
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

            int? userId = await _userRepository.CreateCompanyUserProfileAsync(newUser, newCompany);

            if (userId == null)
            {
                return new CreateUserResult { Status = CreateUserStatus.DatabaseError };
            }

            return new CreateUserResult { Status = CreateUserStatus.Success, UserId = userId };
        }
    }
}