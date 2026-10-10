using _02_Application.DTOs.Vaga;

namespace _02_Application.Enums
{
    public enum VagaOperacaoStatus
    {
        Success,
        UsuarioNaoEncontrado,
        NaoEncontrada,
        SemPermissao
    }

    public record VagaOperacaoResult(
        VagaOperacaoStatus Status,
        VagaDto? Vaga = null);
}