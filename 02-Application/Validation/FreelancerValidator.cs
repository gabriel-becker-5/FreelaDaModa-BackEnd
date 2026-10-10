using _04_Domain.Entities.Identity;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Validation
{
    // Extraído de OrdemServicoService.ValidarFreelancerAsync (#15) para ser reutilizado
    // pela contratação de vaga (#16) sem duplicar a regra.
    public static class FreelancerValidator
    {
        public static async Task<string?> ValidarAsync(IUserRepository userRepository, int freelancerId)
        {
            User? freelancer = await userRepository.GetUserByIdAsync(freelancerId);

            if (freelancer == null)
            {
                return "O freelancer informado não foi encontrado.";
            }

            if (freelancer.IsDeleted)
            {
                return "O freelancer informado não está mais disponível.";
            }

            if (freelancer.Roles == null || !freelancer.Roles.Contains(Roles.Freelancer))
            {
                return "O usuário informado não possui o perfil de freelancer.";
            }

            return null;
        }
    }
}
