using _02_Application.DTOs.Freelancer;
using _04_Domain.Entities.ObjectsFields;

namespace _02_Application.Interfaces
{
    public interface IFreelancerFieldsRepository
    {
        Task<FreelancerProfileDto?> GetProfileFieldsNames(int freelancerId);

        Task<AvailableTime> CreateAvailableTimeAsync(AvailableTime field);

        Task<AverageRevenue> CreateAverageRevenueAsync(AverageRevenue field);

        Task<BusinessType> CreateBusinessTypeAsync(BusinessType field);

        Task<ExperienceYears> CreateExperienceYearsAsync(ExperienceYears field);

        Task<FreelancerPreferences> CreateFreelancerPreferencesAsync(FreelancerPreferences field);

        Task<HowUsuallyArrangeServices> CreateHowUsuallyArrangeServicesAsync(HowUsuallyArrangeServices field);

        Task<WorkshopSize> CreateWorkshopSizeAsync(WorkshopSize field);

        Task<Specialty> CreateSpecialtyAsync(Specialty field);

        Task<OwnMachine> CreateOwnMachineAsync(OwnMachine field);




        Task<bool> AvailableTimeExistsAsync(string availableTimeName);

        Task<bool> AvailableTimeExistsAsync(int availableTimeId);

        Task<bool> BusinessTypeExistsAsync(string businessTypeName);

        Task<bool> BusinessTypeExistsAsync(int businessTypeId);

        Task<bool> ExperienceYearsExistsAsync(string experienceYearsName);

        Task<bool> ExperienceYearsExistsAsync(int experienceYearsId);

        Task<bool> WorkshopSizeExistsAsync(string workshopSizeName);

        Task<bool> WorkshopSizeExistsAsync(int workshopSizeId);

        Task<bool> AverageRevenueExistsAsync(string averageRevenueName);

        Task<bool> AverageRevenueExistsAsync(int averageRevenueId);

        Task<bool> HowUsuallyArrangeServicesExistsAsync(string howUsuallyArrangeServicesName);

        Task<bool> HowUsuallyArrangeServicesExistsAsync(int howUsuallyArrangeServicesId);

        Task<bool> FreelancerPreferencesExistsAsync(string freelancerPreferenceName);

        Task<bool> FreelancerPreferencesExistsAsync(int freelancerPreferenceId);

        Task<bool> SpecialtyExistsAsync(string specialtyRegisterName);

        Task<bool> SpecialtyExistsAsync(int specialtyRegisterId);

        Task<bool> SpecialtyExistsAsync(ICollection<int> specialtyRegisterId);

        Task<bool> OwnMachinesExistsAsync(string OwnMachineName);

        Task<bool> OwnMachinesExistsAsync(int ownMachinesId);

        Task<bool> OwnMachinesExistsAsync(ICollection<int> ownMachinesId);


        // Leitura em lote
        Task<ICollection<AvailableTime>> GetAllAvailableTimes();
        Task<ICollection<AverageRevenue>> GetAllAverageRevenues();
        Task<ICollection<BusinessType>> GetAllBusinessTypes();
        Task<ICollection<ExperienceYears>> GetAllExperienceYears();
        Task<ICollection<FreelancerPreferences>> GetAllFreelancerPreferences();
        Task<ICollection<HowUsuallyArrangeServices>> GetAllHowUsuallyArrangeServices();
        Task<ICollection<Specialty>> GetAllSpecialties();
        Task<ICollection<OwnMachine>> GetAllOwnMachines();
        Task<ICollection<WorkshopSize>> GetAllWorkshopSizes();

        // Leitura por Id
        Task<AvailableTime?> GetAvailableTimeByIdAsync(int id);
        Task<AverageRevenue?> GetAverageRevenueByIdAsync(int id);
        Task<BusinessType?> GetBusinessTypeByIdAsync(int id);
        Task<ExperienceYears?> GetExperienceYearsByIdAsync(int id);
        Task<FreelancerPreferences?> GetFreelancerPreferenceByIdAsync(int id);
        Task<HowUsuallyArrangeServices?> GetHowUsuallyArrangeServiceByIdAsync(int id);
        Task<Specialty?> GetSpecialtyByIdAsync(int id);
        Task<OwnMachine?> GetOwnMachineByIdAsync(int id);
        Task<WorkshopSize?> GetWorkshopSizeByIdAsync(int id);

        // Atualização
        Task UpdateAvailableTimeAsync(AvailableTime availableTime);
        Task UpdateAverageRevenueAsync(AverageRevenue averageRevenue);
        Task UpdateBusinessTypeAsync(BusinessType businessType);
        Task UpdateExperienceYearsAsync(ExperienceYears experienceYears);
        Task UpdateFreelancerPreferencesAsync(FreelancerPreferences freelancerPreferences);
        Task UpdateHowUsuallyArrangeServicesAsync(HowUsuallyArrangeServices howUsuallyArrangeServices);
        Task UpdateOwnMachineAsync(OwnMachine ownMachine);
        Task UpdateSpecialtyAsync(Specialty specialty);
        Task UpdateWorkshopSizeAsync(WorkshopSize workshopSize);

        // Exclusão
        Task<bool?> DeleteAvailableTimeAsync(int id);
        Task<bool?> DeleteAverageRevenueAsync(int id);
        Task<bool?> DeleteBusinessTypeAsync(int id);
        Task<bool?> DeleteExperienceYearAsync(int id);
        Task<bool?> DeleteFreelancerPreferenceAsync(int id);
        Task<bool?> DeleteHowUsuallyArrangeServiceAsync(int id);
        Task<bool?> DeleteSpecialtyAsync(int id);
        Task<bool?> DeleteOwnMachineAsync(int id);
        Task<bool?> DeleteWorkshopSizeAsync(int id);
    }
}
