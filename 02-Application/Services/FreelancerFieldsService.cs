using _02_Application.DTOs.Freelancer;
using _02_Application.Interfaces;
using _04_Domain.Entities.ObjectsFields;

namespace _02_Application.Services
{
    public class FreelancerFieldsService : IFreelancerFieldsService
    {
        private readonly IFreelancerFieldsRepository _freelancerFieldsRepository;

        public FreelancerFieldsService(IFreelancerFieldsRepository freelancerFieldsRepository)
        {
            _freelancerFieldsRepository = freelancerFieldsRepository;
        }

        public async Task<bool> CreateAvailableTimeAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.AvailableTimeExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            AvailableTime newRegister = new()
            {
                AvailableTimeName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateAvailableTimeAsync(newRegister);

            return true;
        }

        public async Task<bool> CreateAverageRevenueAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.AverageRevenueExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            AverageRevenue newRegister = new()
            {
                AverageRevenueName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateAverageRevenueAsync(newRegister);

            return true;
        }

        public async Task<bool> CreateBusinessTypeAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.BusinessTypeExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            BusinessType newRegister = new()
            {
                BusinessTypeName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateBusinessTypeAsync(newRegister);

            return true;
        }

        public async Task<bool> CreateExperienceYearsAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.ExperienceYearsExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            ExperienceYears newRegister = new()
            {
                ExperienceYearsName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateExperienceYearsAsync(newRegister);

            return true;
        }


        public async Task<bool> CreateFreelancerPreferencesAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.FreelancerPreferencesExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            FreelancerPreferences newRegister = new()
            {
                FreelancerPreferencesName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateFreelancerPreferencesAsync(newRegister);

            return true;
        }


        public async Task<bool> CreateHowUsuallyArrangeServicesAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.HowUsuallyArrangeServicesExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            HowUsuallyArrangeServices newRegister = new()
            {
                HowUsuallyArrangeServicesName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateHowUsuallyArrangeServicesAsync(newRegister);

            return true;
        }

        public async Task<bool> CreateWorkshopSizeAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.WorkshopSizeExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            WorkshopSize newRegister = new()
            {
                WorkshopSizeName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateWorkshopSizeAsync(newRegister);

            return true;
        }

        public async Task<bool> CreateSpecialtyAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.SpecialtyExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            Specialty newRegister = new()
            {
                SpecialtyName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateSpecialtyAsync(newRegister);

            return true;
        }

        public async Task<bool> CreateOwnMachineAsync(FieldsFreelancerDto field)
        {
            bool result = await _freelancerFieldsRepository.OwnMachinesExistsAsync(field.RegisterName);

            if (result)
            {
                return false;
            }

            OwnMachine newRegister = new()
            {
                OwnMachineName = field.RegisterName
            };

            await _freelancerFieldsRepository.CreateOwnMachineAsync(newRegister);

            return true;
        }


        public async Task<FreelancerProfileDto?> GetProfileFieldsNames(int freelancerId)
        {
            return await _freelancerFieldsRepository.GetProfileFieldsNames(freelancerId);
        }










        public async Task<ICollection<FieldsFreelancerDto>> GetAllAvailableTimes()
        {
            ICollection<AvailableTime> result = await _freelancerFieldsRepository.GetAllAvailableTimes();

            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.AvailableTimeName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }

        public async Task<ICollection<FieldsFreelancerDto>> GetAllAverageRevenues()
        {
            ICollection<AverageRevenue> result = await _freelancerFieldsRepository.GetAllAverageRevenues();

            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.AverageRevenueName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }

        public async Task<ICollection<FieldsFreelancerDto>> GetAllBusinessTypes()
        {
            ICollection<BusinessType> result = await _freelancerFieldsRepository.GetAllBusinessTypes();

            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.BusinessTypeName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }

        public async Task<ICollection<FieldsFreelancerDto>> GetAllExperienceYears()
        {
            ICollection<ExperienceYears> result = await _freelancerFieldsRepository.GetAllExperienceYears();

            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.ExperienceYearsName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }

        public async Task<ICollection<FieldsFreelancerDto>> GetAllFreelancerPreferences()
        {
            ICollection<FreelancerPreferences> result = await _freelancerFieldsRepository.GetAllFreelancerPreferences();

            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.FreelancerPreferencesName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }

        public async Task<ICollection<FieldsFreelancerDto>> GetAllSpecialties()
        {
            ICollection<Specialty> result = await _freelancerFieldsRepository.GetAllSpecialties();

            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.SpecialtyName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }

        public async Task<ICollection<FieldsFreelancerDto>> GetAllHowUsuallyArrangeServices()
        {
            ICollection<HowUsuallyArrangeServices> result = await _freelancerFieldsRepository.GetAllHowUsuallyArrangeServices();

            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.HowUsuallyArrangeServicesName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }

        public async Task<ICollection<FieldsFreelancerDto>> GetAllOwnMachines()
        {
            ICollection<OwnMachine> result = await _freelancerFieldsRepository.GetAllOwnMachines();

            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.OwnMachineName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }

        public async Task<ICollection<FieldsFreelancerDto>> GetAllWorkshopSizes()
        {
            ICollection<WorkshopSize> result = await _freelancerFieldsRepository.GetAllWorkshopSizes();
            List<FieldsFreelancerDto> fieldsDto = [];

            foreach (var field in result)
            {
                FieldsFreelancerDto newFieldDto = new()
                {
                    RegisterName = field.WorkshopSizeName
                };

                fieldsDto.Add(newFieldDto);
            }

            return fieldsDto;
        }





        public async Task<FieldsFreelancerDto?> GetAvailableTimeByIdAsync(int id)
        {
            AvailableTime? result = await _freelancerFieldsRepository.GetAvailableTimeByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.AvailableTimeName
            };

            return fieldDto;
        }

        public async Task<FieldsFreelancerDto?> GetAverageRevenueByIdAsync(int id)
        {
            AverageRevenue? result = await _freelancerFieldsRepository.GetAverageRevenueByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.AverageRevenueName
            };

            return fieldDto;
        }

        public async Task<FieldsFreelancerDto?> GetBusinessTypeByIdAsync(int id)
        {
            BusinessType? result = await _freelancerFieldsRepository.GetBusinessTypeByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.BusinessTypeName
            };

            return fieldDto;
        }

        public async Task<FieldsFreelancerDto?> GetExperienceYearsByIdAsync(int id)
        {
            ExperienceYears? result = await _freelancerFieldsRepository.GetExperienceYearsByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.ExperienceYearsName
            };

            return fieldDto;
        }

        public async Task<FieldsFreelancerDto?> GetFreelancerPreferencesByIdAsync(int id)
        {
            FreelancerPreferences? result = await _freelancerFieldsRepository.GetFreelancerPreferenceByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.FreelancerPreferencesName
            };

            return fieldDto;
        }

        public async Task<FieldsFreelancerDto?> GetSpecialtyByIdAsync(int id)
        {
            Specialty? result = await _freelancerFieldsRepository.GetSpecialtyByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.SpecialtyName
            };

            return fieldDto;
        }

        public async Task<FieldsFreelancerDto?> GetHowUsuallyArrangeServicesByIdAsync(int id)
        {
            HowUsuallyArrangeServices? result = await _freelancerFieldsRepository.GetHowUsuallyArrangeServiceByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.HowUsuallyArrangeServicesName
            };

            return fieldDto;
        }

        public async Task<FieldsFreelancerDto?> GetOwnMachineByIdAsync(int id)
        {
            OwnMachine? result = await _freelancerFieldsRepository.GetOwnMachineByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.OwnMachineName
            };

            return fieldDto;
        }

        public async Task<FieldsFreelancerDto?> GetWorkshopSizeByIdAsync(int id)
        {
            WorkshopSize? result = await _freelancerFieldsRepository.GetWorkshopSizeByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            FieldsFreelancerDto fieldDto = new()
            {
                RegisterName = result.WorkshopSizeName
            };

            return fieldDto;
        }




        public async Task<bool> UpdateAvailableTimeAsync(int id, FieldsFreelancerDto field)
        {
            AvailableTime? result = await _freelancerFieldsRepository.GetAvailableTimeByIdAsync(id);

            if (result != null)
            {
                result.AvailableTimeName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateAvailableTimeAsync(result);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAverageRevenueAsync(int id, FieldsFreelancerDto field)
        {
            AverageRevenue? result = await _freelancerFieldsRepository.GetAverageRevenueByIdAsync(id);

            if (result != null)
            {
                result.AverageRevenueName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateAverageRevenueAsync(result);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateBusinessTypeAsync(int id, FieldsFreelancerDto field)
        {
            BusinessType? result = await _freelancerFieldsRepository.GetBusinessTypeByIdAsync(id);

            if (result != null)
            {
                result.BusinessTypeName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateBusinessTypeAsync(result);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateExperienceYearsAsync(int id, FieldsFreelancerDto field)
        {
            ExperienceYears? result = await _freelancerFieldsRepository.GetExperienceYearsByIdAsync(id);

            if (result != null)
            {
                result.ExperienceYearsName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateExperienceYearsAsync(result);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateFreelancerPreferencesAsync(int id, FieldsFreelancerDto field)
        {
            FreelancerPreferences? result = await _freelancerFieldsRepository.GetFreelancerPreferenceByIdAsync(id);

            if (result != null)
            {
                result.FreelancerPreferencesName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateFreelancerPreferencesAsync(result);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateHowUsuallyArrangeServicesAsync(int id, FieldsFreelancerDto field)
        {
            HowUsuallyArrangeServices? result = await _freelancerFieldsRepository.GetHowUsuallyArrangeServiceByIdAsync(id);

            if (result != null)
            {
                result.HowUsuallyArrangeServicesName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateHowUsuallyArrangeServicesAsync(result);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateOwnMachineAsync(int id, FieldsFreelancerDto field)
        {
            OwnMachine? result = await _freelancerFieldsRepository.GetOwnMachineByIdAsync(id);

            if (result != null)
            {
                result.OwnMachineName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateOwnMachineAsync(result);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateSpecialtyAsync(int id, FieldsFreelancerDto field)
        {
            Specialty? result = await _freelancerFieldsRepository.GetSpecialtyByIdAsync(id);

            if (result != null)
            {
                result.SpecialtyName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateSpecialtyAsync(result);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateWorkshopSizeAsync(int id, FieldsFreelancerDto field)
        {
            WorkshopSize? result = await _freelancerFieldsRepository.GetWorkshopSizeByIdAsync(id);

            if (result != null)
            {
                result.WorkshopSizeName = field.RegisterName;
                await _freelancerFieldsRepository.UpdateWorkshopSizeAsync(result);
                
                return true;
            }

            return false;
        }


        public async Task<bool?> DeleteAvailableTimeAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteAvailableTimeAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> DeleteAverageRevenueAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteAverageRevenueAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> DeleteBusinessTypeAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteBusinessTypeAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> DeleteExperienceYearAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteExperienceYearAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> DeleteFreelancerPreferenceAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteFreelancerPreferenceAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> DeleteHowUsuallyArrangeServiceAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteHowUsuallyArrangeServiceAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> DeleteOwnMachineAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteOwnMachineAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> DeleteSpecialtyAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteSpecialtyAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> DeleteWorkshopSizeAsync(int id)
        {
            bool? result = await _freelancerFieldsRepository.DeleteWorkshopSizeAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }
    }
}