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
using Elekto.BrazilianDocuments;

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

        // Criar usuários
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

            if (!Cpf.IsValid(dto.LegalResponsibleDocument))
            {
                return new CreateUserResult { Status = CreateUserStatus.InvalidCPF };
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
                Roles = new List<Roles> { Roles.Freelancer },
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                BirthDate = dto.BirthDate
            };

            FreelancerProfile newFreelancer = new()
            {
                AvailableTimeId = (AvailableTime)dto.AvailableTimeId,
                ExperienceYearsId = (ExperienceYears)dto.ExperienceYearsId,
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

            if (!Cpf.IsValid(dto.LegalResponsibleDocument))
            {
                return new CreateUserResult { Status = CreateUserStatus.InvalidCPF };
            }

            if (await _userRepository.IsCnpjRegistered(dto.CompanyRegistrationDocument))
            {
                return new CreateUserResult { Status = CreateUserStatus.CnpjInUse };
            }

            if (!Cnpj.IsValid(dto.CompanyRegistrationDocument))
            {
                return new CreateUserResult { Status = CreateUserStatus.InvalidCNPJ };
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
                Roles = new List<Roles> { Roles.Company },
                PasswordHash = _passwordHasher.HashPassword(dto.Password)
            };

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
            page = PageGuard(page);
            pageSize = PageSizeGuard(pageSize);

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

        public async Task<PagedResult<GetFreelancerDto>> GetAllFreelancersAsync(int page, int pageSize)
        {
            page = PageGuard(page);
            pageSize = PageSizeGuard(pageSize);
            int skip = (page - 1) * pageSize;

            ICollection<User> users = await _userRepository.GetAllFreelancersAsync(skip, pageSize);
            int total = await _userRepository.CountFreelancersAsync();

            List<GetFreelancerDto> items = [];

            foreach (User user in users)
            {
                if (user.FreelancerProfile != null)
                {
                    items.Add(MapToFreelancerDto(user));
                }
            }

            return new PagedResult<GetFreelancerDto>
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

            if (user.FreelancerProfile == null)
            {
                return null;
            }

            return MapToFreelancerDto(user);
        }

        public async Task<FreelancerPublicProfileDto?> GetFreelancerPublicProfileByIdAsync(int userId)
        {
            User? user = await _userRepository.GetFreelancerProfileAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (user.FreelancerProfile == null)
            {
                return null;
            }

            return MapToFreelancerPublicProfileDto(user);
        }

        public async Task<GetCompanyDto?> GetCompanyProfileByIdAsync(int userId)
        {
            User? user = await _userRepository.GetCompanyProfileAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (user.CompanyProfile == null)
            {
                return null;
            }

            return MapToCompanyDto(user);
        }

        public async Task<CompanyPublicProfileDto?> GetCompanyPublicProfileByIdAsync(int userId)
        {
            User? user = await _userRepository.GetCompanyProfileAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (user.CompanyProfile == null)
            {
                return null;
            }

            return MapToCompanyPublicProfileDto(user);
        }

        // Atualização de perfil
        public async Task<ProfileUpdateResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto, int userId)
        {
            User? user = await _userRepository.GetFreelancerProfileAsync(userId);
            if (user == null)
            {
                return ProfileUpdateResult.NotFound;
            }

            ProfileUpdateResult emailValidation = await ValidateEmail(dto.Email, user.Email);
            if (emailValidation == ProfileUpdateResult.Success)
            {
                user.Email = dto.Email;
            }
            else
            {
                return emailValidation;
            }

            ProfileUpdateResult cpfValidation = await ValidarCpf(dto.LegalResponsibleDocument, user.LegalResponsibleDocument);
            if (cpfValidation == ProfileUpdateResult.Success)
            {
                user.LegalResponsibleDocument = dto.LegalResponsibleDocument;
            }
            else
            {
                return cpfValidation;
            }

            ApplyUserChanges(user, dto);

            if (dto.BirthDate.HasValue
                    && dto.BirthDate != user.BirthDate) // se tem valor e é diferente do atual, segue.
            {
                if (dto.BirthDate.Value != default(DateTime)
                        && dto.BirthDate.Value < DateTime.UtcNow) // se dto não é padrão e dto é menor igual agora
                {
                    user.BirthDate = (DateTime)dto.BirthDate;
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

            ProfileUpdateResult emailValidation = await ValidateEmail(dto.Email, user.Email);
            if (emailValidation == ProfileUpdateResult.Success)
            {
                user.Email = dto.Email;
            }
            else
            {
                return emailValidation;
            }

            ProfileUpdateResult cpfValidation = await ValidarCpf(dto.LegalResponsibleDocument, user.LegalResponsibleDocument);
            if (cpfValidation == ProfileUpdateResult.Success)
            {
                user.LegalResponsibleDocument = dto.LegalResponsibleDocument;
            }
            else
            {
                return cpfValidation;
            }

            ProfileUpdateResult cnpjValidation = await ValidarCNPJ(dto.CompanyRegistrationDocument, user.CompanyProfile.CompanyRegistrationDocument);
            if (cnpjValidation == ProfileUpdateResult.Success)
            {
                user.CompanyProfile.CompanyRegistrationDocument = dto.CompanyRegistrationDocument;
            }
            else
            {
                return cnpjValidation;
            }

            ApplyUserChanges(user, dto);

            if (!string.IsNullOrEmpty(dto.LegalName)
                && !string.Equals(dto.LegalName, user.CompanyProfile.LegalName, StringComparison.OrdinalIgnoreCase))
            {
                user.CompanyProfile.LegalName = dto.LegalName;
            }

            if (!string.IsNullOrEmpty(dto.CompanyName)
                && !string.Equals(dto.CompanyName, user.CompanyProfile.CompanyName, StringComparison.OrdinalIgnoreCase))
            {
                user.CompanyProfile.CompanyName = dto.CompanyName;
            }

            if (!string.IsNullOrEmpty(dto.CoreBusiness)
                && !string.Equals(dto.CoreBusiness, user.CompanyProfile.CoreBusiness, StringComparison.OrdinalIgnoreCase))
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
        public async Task<ProfileUpdateResult> ValidarCpf(string dtoDocument, string userDocument)
        {
            if (string.IsNullOrEmpty(dtoDocument)
                || string.Equals(dtoDocument, userDocument, StringComparison.OrdinalIgnoreCase)
                || !Cpf.IsValid(dtoDocument))
            {
                return ProfileUpdateResult.InvalidCPF;
            }

            if (await _userRepository.IsCpfRegistered(dtoDocument))
            {
                return ProfileUpdateResult.DocumentInUse;
            }

            return ProfileUpdateResult.Success;
        }

        public async Task<ProfileUpdateResult> ValidarCNPJ(string dtoDocument, string userDocument)
        {
            if (string.IsNullOrEmpty(dtoDocument)
                || string.Equals(dtoDocument, userDocument, StringComparison.OrdinalIgnoreCase)
                || !Cnpj.IsValid(dtoDocument))
            {
                return ProfileUpdateResult.InvalidCNPJ;
            }

            if (await _userRepository.IsCnpjRegistered(dtoDocument))
            {
                return ProfileUpdateResult.DocumentInUse;
            }

            return ProfileUpdateResult.Success;
        }

        private static void ApplyUserChanges(User user, UpdateUserDto dto)
        {
            if (!string.IsNullOrEmpty(dto.LegalResponsibleFullName)
                && !string.Equals(dto.LegalResponsibleFullName, user.LegalResponsibleFullName, StringComparison.OrdinalIgnoreCase))
            {
                user.LegalResponsibleFullName = dto.LegalResponsibleFullName;
            }

            if (!string.IsNullOrEmpty(dto.LegalResponsibleDocument)
                && !string.Equals(dto.LegalResponsibleDocument, user.LegalResponsibleDocument, StringComparison.OrdinalIgnoreCase))
            {
                user.LegalResponsibleDocument = dto.LegalResponsibleDocument;
            }

            if (!string.IsNullOrEmpty(dto.Email)
                && !string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.ContactNumber)
                && !string.Equals(dto.ContactNumber, user.ContactNumber, StringComparison.OrdinalIgnoreCase))
            {
                user.ContactNumber = dto.ContactNumber;
            }

            if (!string.IsNullOrEmpty(dto.PostalCode)
                && !string.Equals(dto.PostalCode, user.PostalCode, StringComparison.OrdinalIgnoreCase))
            {
                user.PostalCode = dto.PostalCode;
            }

            if (!string.IsNullOrEmpty(dto.Address)
                && !string.Equals(dto.Address, user.Address, StringComparison.OrdinalIgnoreCase))
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
                && !string.Equals(dto.Neighborhood, user.Neighborhood, StringComparison.OrdinalIgnoreCase))
            {
                user.Neighborhood = dto.Neighborhood;
            }

            if (dto.AdditionalAddressInfo == "")
            {
                user.AdditionalAddressInfo = null; // permite limpar o complemento
            }
            else if (dto.AdditionalAddressInfo != null
                && (user.AdditionalAddressInfo == null
                    || !string.Equals(dto.AdditionalAddressInfo, user.AdditionalAddressInfo, StringComparison.OrdinalIgnoreCase)))
            {
                user.AdditionalAddressInfo = dto.AdditionalAddressInfo;
            }

            if (!string.IsNullOrEmpty(dto.City)
                && !string.Equals(dto.City, user.City, StringComparison.OrdinalIgnoreCase))
            {
                user.City = dto.City;
            }

            if (!string.IsNullOrEmpty(dto.State)
                && !string.Equals(dto.State, user.State, StringComparison.OrdinalIgnoreCase))
            {
                user.State = dto.State;
            }

            if (!string.IsNullOrEmpty(dto.PublicProfileDescription)
                && (dto.PublicProfileDescription == null
                    || !string.Equals(dto.PublicProfileDescription, user.PublicProfileDescription, StringComparison.OrdinalIgnoreCase)))
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
                AdditionalAddressInfo = user.AdditionalAddressInfo,
                ProfileImageUrl = GetProfileImageUrl(user)
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
                BirthDate = user.BirthDate,
                AvailableTimeName = user.FreelancerProfile.AvailableTimeId.ToString().Replace("_", " "),
                ExperienceYearsName = user.FreelancerProfile.ExperienceYearsId.ToString().Replace("_", " "),
                OwnMachineNames = user.FreelancerProfile.OwnMachinesIds
                                    .Select(id => id
                                    .ToString()
                                    .Replace("_", " "))
                                    .ToList(),
                SpecialtyNames = user.FreelancerProfile.SpecialtiesIds
                                    .Select(id => id
                                    .ToString()
                                    .Replace("_", " "))
                                    .ToList(),
                ProfileImageUrl = GetProfileImageUrl(user)
            };
        }

        private static FreelancerPublicProfileDto MapToFreelancerPublicProfileDto(User user)
        {
            return new FreelancerPublicProfileDto
            {
                LegalResponsibleFullName = user.LegalResponsibleFullName,
                City = user.City,
                State = user.State,
                IsVerified = user.IsVerified,
                PublicProfileDescription = user.PublicProfileDescription,
                AvailableTimeName = user.FreelancerProfile.AvailableTimeId.ToString().Replace("_", " "),
                ExperienceYearsName = user.FreelancerProfile.ExperienceYearsId.ToString().Replace("_", " "),
                OwnMachineNames = user.FreelancerProfile.OwnMachinesIds
                                    .Select(id => id
                                    .ToString()
                                    .Replace("_", " "))
                                    .ToList(),
                SpecialtyNames = user.FreelancerProfile.SpecialtiesIds
                                    .Select(id => id
                                    .ToString()
                                    .Replace("_", " "))
                                    .ToList(),
                ProfileImageUrl = GetProfileImageUrl(user)
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
                CoreBusiness = user.CompanyProfile.CoreBusiness,
                ProfileImageUrl = GetProfileImageUrl(user)
            };
        }

        private static CompanyPublicProfileDto MapToCompanyPublicProfileDto(User user)
        {
            return new CompanyPublicProfileDto
            {
                CompanyName = user.CompanyProfile.CompanyName,
                City = user.City,
                State = user.State,
                IsVerified = user.IsVerified,
                PublicProfileDescription = user.PublicProfileDescription,
                CoreBusiness = user.CompanyProfile.CoreBusiness,
                ProfileImageUrl = GetProfileImageUrl(user)
            };
        }

        public async Task<ProfileUpdateResult> ValidateEmail(string dtoEmail, string userEmail)
        {
            if (string.IsNullOrEmpty(dtoEmail)
                || string.Equals(dtoEmail, userEmail, StringComparison.OrdinalIgnoreCase))
            {
                return ProfileUpdateResult.InvalidEmail;
            }

            if (await _userRepository.IsEmailRegistered(dtoEmail))
            {
                return ProfileUpdateResult.EmailInUse;
            }

            return ProfileUpdateResult.Success;
        }

        private static int PageGuard(int page)
        {
            return Math.Clamp(page, 1, 1000);
        }

        private static int PageSizeGuard(int pageSize)
        {
            return Math.Clamp(pageSize, 1, 50);
        }

        private static string? GetProfileImageUrl(User user)
        {
            return user.ProfileImageKey == null
                ? null
                : $"/api/v1/User/{user.Id}/imagem-perfil";
        }
    }
}