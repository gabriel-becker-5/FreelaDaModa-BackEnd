namespace _02_Application.DTOs.Dashboard
{
    public record ConversaPreviewDto(
        int ContatoId,
        string ContatoNome,
        string UltimaMensagem,
        DateTime DataUltimaMensagem);
}
