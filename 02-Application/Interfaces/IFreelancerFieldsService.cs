using _02_Application.DTOs;

namespace _02_Application.Interfaces
{
    public interface IFreelancerFieldsService
    {
        Task<ICollection<IdLabelDto>> GetAll<T>() where T : struct, Enum;
    }
}