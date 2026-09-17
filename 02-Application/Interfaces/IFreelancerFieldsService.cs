using _02_Application.DTOs.Freelancer;

namespace _02_Application.Interfaces
{
    public interface IFreelancerFieldsService
    {
        Task<ICollection<IdLabelDto>> GetAll<T>() where T : struct, Enum;
    }
}