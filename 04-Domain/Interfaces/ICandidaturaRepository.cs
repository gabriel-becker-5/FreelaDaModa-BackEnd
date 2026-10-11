using _04_Domain.Entities;
using _04_Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _04_Domain.Interfaces
{
    public interface ICandidaturaRepository
    {
        // Retorna a vaga se ela existir e não estiver excluída; senão, null
        Task<Vaga?> ObterVagaNaoExcluidaAsync(int vagaId);

        Task<bool> ExisteCandidaturaAsync(int vagaId, int freelancerId);

        Task<Candidatura> CreateAsync(Candidatura candidatura);

        // Retorna as candidaturas do freelancer já com a Vaga carregada
        Task<ICollection<Candidatura>> GetByFreelancerAsync(int freelancerId);

        // Retorna as candidaturas filtradas já com a Vaga carregada
        Task<ICollection<Candidatura>> GetFiltradasAsync(
            int? freelancerId,
            int? empresaId,
            int? vagaId,
            StatusCandidatura? status);

        // Busca a candidatura já com a Vaga carregada (a vaga pode estar encerrada ou excluída)
        // ATENÇÃO: retorna a entidade rastreada, pois é alterada pelo CandidaturaService
        Task<Candidatura?> GetComVagaAsync(int vagaId, int freelancerId);

        Task UpdateAsync(Candidatura candidatura);

        // Usados pelo Dashboard
        Task<List<Candidatura>> ListByFreelancerIdAsync(int freelancerId);

        Task<List<Candidatura>> ListByEmpresaIdAsync(int empresaUserId);
    }
}