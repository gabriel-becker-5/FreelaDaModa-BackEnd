using _04_Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _04_Domain.Interfaces
{
    public interface ICandidaturaRepository
    {
        Task<List<Candidatura>> ListByFreelancerIdAsync(int freelancerId);

        Task<List<Candidatura>> ListByEmpresaIdAsync(int empresaUserId);
    }
}
