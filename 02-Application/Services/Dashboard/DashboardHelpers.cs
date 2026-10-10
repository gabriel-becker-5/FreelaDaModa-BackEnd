using _02_Application.DTOs.Dashboard;
using _04_Domain.Entities;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Services.Dashboard
{
    internal static class DashboardHelpers
    {
        // OrdemServico.Status é string no banco (contrato com o front exige os 3 textos exatos),
        // mas a leitura usa o parse tolerante de StatusOrdemServicoExtensions (aceita variações e
        // valores legados). Nulo/vazio é tratado como "Em andamento" (ver D3 do plano).
        public static bool EstaEmAndamento(OrdemServico os)
        {
            if (string.IsNullOrWhiteSpace(os.Status))
            {
                return true;
            }

            return StatusOrdemServicoExtensions.TryParse(os.Status, out StatusOrdemServico status)
                && status == StatusOrdemServico.EmAndamento;
        }

        public static bool EstaConcluida(OrdemServico os)
        {
            return StatusOrdemServicoExtensions.TryParse(os.Status, out StatusOrdemServico status)
                && status == StatusOrdemServico.Concluida;
        }

        public static string MapStatusCandidatura(StatusCandidatura status)
        {
            return status switch
            {
                StatusCandidatura.Pendente => "Em análise",
                StatusCandidatura.Aceita => "Selecionado",
                StatusCandidatura.Rejeitada => "Rejeitado",
                _ => status.ToString()
            };
        }

        public static async Task<string> ResolverNomeEmpresaAsync(IUserRepository userRepository, int empresaUserId)
        {
            var empresa = await userRepository.GetCompanyProfileAsync(empresaUserId);
            return empresa?.CompanyProfile?.CompanyName
                ?? empresa?.CompanyProfile?.LegalName
                ?? "Confecção";
        }

        public static async Task<string> ResolverNomeFreelancerAsync(IUserRepository userRepository, int freelancerId)
        {
            var freelancer = await userRepository.GetUserByIdAsync(freelancerId);
            return freelancer?.LegalResponsibleFullName ?? "Freelancer";
        }

        public static async Task<List<ConversaPreviewDto>> MontarPreviewConversasAsync(
            IUserRepository userRepository,
            int usuarioId,
            List<Mensagem> mensagens,
            int quantidade)
        {
            var grupos = mensagens
                .GroupBy(m => m.RemetenteId == usuarioId ? m.DestinatarioId : m.RemetenteId)
                .Select(g => g.OrderByDescending(m => m.DataEnvio).First())
                .OrderByDescending(m => m.DataEnvio)
                .Take(quantidade)
                .ToList();

            var resultado = new List<ConversaPreviewDto>();
            foreach (var mensagem in grupos)
            {
                int contatoId = mensagem.RemetenteId == usuarioId ? mensagem.DestinatarioId : mensagem.RemetenteId;
                var contato = await userRepository.GetUserByIdAsync(contatoId);

                resultado.Add(new ConversaPreviewDto(
                    contatoId,
                    contato?.LegalResponsibleFullName ?? "Contato",
                    mensagem.Conteudo,
                    mensagem.DataEnvio));
            }

            return resultado;
        }
    }
}
