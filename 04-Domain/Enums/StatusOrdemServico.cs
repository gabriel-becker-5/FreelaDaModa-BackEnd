using System;
using System.Globalization;
using System.Text;

namespace _04_Domain.Enums
{
    public enum StatusOrdemServico
    {
        EmAndamento = 1,
        Concluida = 2,
        Cancelada = 3
    }

    public static class StatusOrdemServicoExtensions
    {
        public const string TextoEmAndamento = "Em andamento";
        public const string TextoConcluida = "Concluída";
        public const string TextoCancelada = "Cancelada";

        public static string ParaTexto(this StatusOrdemServico status)
        {
            return status switch
            {
                StatusOrdemServico.EmAndamento => TextoEmAndamento,
                StatusOrdemServico.Concluida => TextoConcluida,
                StatusOrdemServico.Cancelada => TextoCancelada,
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Status de ordem de serviço desconhecido.")
            };
        }

        // Tolerante a caixa/acento e a valores legados gravados antes do enum existir.
        public static bool TryParse(string? texto, out StatusOrdemServico status)
        {
            status = StatusOrdemServico.EmAndamento;

            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            switch (NormalizarTexto(texto))
            {
                case "em andamento":
                    status = StatusOrdemServico.EmAndamento;
                    return true;
                case "concluida":
                case "concluido":
                case "finalizada":
                case "finalizado":
                    status = StatusOrdemServico.Concluida;
                    return true;
                case "cancelada":
                case "cancelado":
                    status = StatusOrdemServico.Cancelada;
                    return true;
                default:
                    return false;
            }
        }

        private static string NormalizarTexto(string texto)
        {
            string formaDecomposta = texto.Trim().Normalize(NormalizationForm.FormD);
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
    }
}
