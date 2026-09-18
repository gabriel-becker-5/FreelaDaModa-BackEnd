using _02_Application.DTOs;
using _02_Application.Interfaces;

namespace _02_Application.Services
{
    public class FreelancerFieldsService : IFreelancerFieldsService
    {
        public async Task<ICollection<IdLabelDto>> GetAll<T>() where T : struct, Enum
        {
            return Enum.GetValues<T>()
                .Select(e => new IdLabelDto
                {
                    Id = Convert.ToInt32(e),
                    Label = e.ToString().Replace("_", " ")
                })
                .ToList();
        }
    }
}