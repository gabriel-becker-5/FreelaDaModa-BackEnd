using System.Globalization;
using System.Text;
using _02_Application.DTOs.Dashboard;
using _04_Domain.Entities;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Services.Dashboard
{
    internal static class DashboardHelpers
    {
        // OrdemServico.Status é texto livre (sem enum/validação). O conjunto de valores aceitos
        // abaixo ("em andamento", "concluida"/"concluido"/"finalizada"/"finalizado") deve ser
        // confirmado com quem cuida do módulo de Ordem de Serviço — o ideal é esse campo virar
        // um enum (como StatusVaga/StatusCandidatura) para não depender de comparação de texto.
        private static string NormalizarStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return string.Empty;
            }

            string formaDecomposta = status.Trim().Normalize(NormalizationForm.FormD);
            var semAcentos = new StringBuilder(formaDecomposta.Length);

            foreach (char c in formaDecomposta)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    semAcentos.Append(c);
                }
            }

            return semAcentos.ToString().ToLowerInvariant();
        }

        // OrdemServico.Status é texto livre e nulo por padrão — null é tratado como "Em andamento" (ver D3 do plano).
        public static bool EstaEmAndamento(OrdemServico os)
        {
            string statusNormalizado = NormalizarStatus(os.Status);
            return statusNormalizado.Length == 0 || statusNormalizado == "em andamento";
        }

        public static bool EstaConcluida(OrdemServico os)
        {
            string statusNormalizado = NormalizarStatus(os.Status);
            return statusNormalizado is "concluida" or "concluido" or "finalizada" or "finalizado";
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
