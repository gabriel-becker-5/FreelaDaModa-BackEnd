using _02_Application.DTOs.Freelancer;
using _04_Domain.Entities.ObjectsFields;

namespace _02_Application.Interfaces
{
    public interface IFreelancerFieldsService
    {
        Task<FreelancerProfileDto?> GetProfileFieldsNames(int freelancerId);

        Task<bool> CreateAvailableTimeAsync(FieldsFreelancerDto field);

        Task<bool> CreateAverageRevenueAsync(FieldsFreelancerDto field);

        Task<bool> CreateBusinessTypeAsync(FieldsFreelancerDto field);

        Task<bool> CreateExperienceYearsAsync(FieldsFreelancerDto field);

        Task<bool> CreateFreelancerPreferencesAsync(FieldsFreelancerDto field);

        Task<bool> CreateHowUsuallyArrangeServicesAsync(FieldsFreelancerDto field);

        Task<bool> CreateWorkshopSizeAsync(FieldsFreelancerDto field);

        Task<bool> CreateSpecialtyAsync(FieldsFreelancerDto field);

        Task<bool> CreateOwnMachineAsync(FieldsFreelancerDto field);









        // Leitura em lote
        Task<ICollection<FieldsFreelancerDto>> GetAllAvailableTimes();
        Task<ICollection<FieldsFreelancerDto>> GetAllAverageRevenues();
        Task<ICollection<FieldsFreelancerDto>> GetAllBusinessTypes();
        Task<ICollection<FieldsFreelancerDto>> GetAllExperienceYears();
        Task<ICollection<FieldsFreelancerDto>> GetAllFreelancerPreferences();
        Task<ICollection<FieldsFreelancerDto>> GetAllSpecialties();
        Task<ICollection<FieldsFreelancerDto>> GetAllHowUsuallyArrangeServices();
        Task<ICollection<FieldsFreelancerDto>> GetAllOwnMachines();
        Task<ICollection<FieldsFreelancerDto>> GetAllWorkshopSizes();

        // Leitura por Id
        Task<FieldsFreelancerDto?> GetAvailableTimeByIdAsync(int id);
        Task<FieldsFreelancerDto?> GetAverageRevenueByIdAsync(int id);
        Task<FieldsFreelancerDto?> GetBusinessTypeByIdAsync(int id);
        Task<FieldsFreelancerDto?> GetExperienceYearsByIdAsync(int id);
        Task<FieldsFreelancerDto?> GetFreelancerPreferencesByIdAsync(int id);
        Task<FieldsFreelancerDto?> GetSpecialtyByIdAsync(int id);
        Task<FieldsFreelancerDto?> GetHowUsuallyArrangeServicesByIdAsync(int id);
        Task<FieldsFreelancerDto?> GetOwnMachineByIdAsync(int id);
        Task<FieldsFreelancerDto?> GetWorkshopSizeByIdAsync(int id);

        // Atualização
        Task<bool> UpdateAvailableTimeAsync(int id, FieldsFreelancerDto field);
        Task<bool> UpdateAverageRevenueAsync(int id, FieldsFreelancerDto field);
        Task<bool> UpdateBusinessTypeAsync(int id, FieldsFreelancerDto field);
        Task<bool> UpdateExperienceYearsAsync(int id, FieldsFreelancerDto field);
        Task<bool> UpdateFreelancerPreferencesAsync(int id, FieldsFreelancerDto field);
        Task<bool> UpdateHowUsuallyArrangeServicesAsync(int id, FieldsFreelancerDto field);
        Task<bool> UpdateOwnMachineAsync(int id, FieldsFreelancerDto field);
        Task<bool> UpdateSpecialtyAsync(int id, FieldsFreelancerDto field);
        Task<bool> UpdateWorkshopSizeAsync(int id, FieldsFreelancerDto field);

        // Exclusão
        Task<bool?> DeleteAvailableTimeAsync(int id);
        Task<bool?> DeleteAverageRevenueAsync(int id);
        Task<bool?> DeleteBusinessTypeAsync(int id);
        Task<bool?> DeleteExperienceYearAsync(int id);
        Task<bool?> DeleteFreelancerPreferenceAsync(int id);
        Task<bool?> DeleteHowUsuallyArrangeServiceAsync(int id);
        Task<bool?> DeleteOwnMachineAsync(int id);
        Task<bool?> DeleteSpecialtyAsync(int id);
        Task<bool?> DeleteWorkshopSizeAsync(int id);

    }
}