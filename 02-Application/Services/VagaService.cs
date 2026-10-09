using _02_Application.DTOs.Vaga;
using _02_Application.Interfaces;
using _02_Application.Mappings;
using _03_Infrastructure.Repositories.Vaga;
using _04_Domain.Entities;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Services;

public class VagaService : IVagaService
{
    private readonly IVagaRepository _vagaRepository;
    private readonly IUserRepository _userRepository;

    public VagaService(IVagaRepository vagaRepository, IUserRepository userRepository)
    {
        _vagaRepository = vagaRepository;
        _userRepository = userRepository;
    }

    public async Task<RespostavagaJson> RegistrarAsync(RequisicaoRegistrarVagaJson requisicao, int usuarioId)
    {
        var user = await _userRepository.GetUserByIdAsync(usuarioId);
        if (user == null || user.IsDeleted)
        {
            throw new UnauthorizedAccessException("Usuário não encontrado.");
        }

        if (!requisicao.Especialidade.HasValue ||
            !requisicao.Modalidade.HasValue ||
            !requisicao.PrazoConclusao.HasValue)
        {
            throw new ArgumentException(
                "Especialidade, modalidade e prazo de conclusão são obrigatórios.",
                nameof(requisicao));
        }

        var entidade = new Vaga
        {
            Titulo = requisicao.Titulo,
            Especialidade = requisicao.Especialidade.Value,
            Modalidade = requisicao.Modalidade.Value,
            Cidade = requisicao.Cidade,
            Estado = requisicao.Estado,
            Orcamento = requisicao.Orcamento,
            PrazoConclusao = requisicao.PrazoConclusao.Value,
            DataPublicacao = DateTime.UtcNow,
            Descricao = requisicao.Descricao,
            Status = StatusVaga.Aberta,
            UsuarioId = user.Id
        };

        await _vagaRepository.AdicionarAsync(entidade);
        return entidade.ParaRespostaJson();
    }

    public async Task<IEnumerable<RespostavagaJson>> ObterTodasAsync()
    {
        var list = await _vagaRepository.ObterTodasAsync();
        return list.ParaRespostaJson();
    }

    public async Task<IEnumerable<RespostavagaJson>> ObterComFiltrosAsync(int? usuarioId, string? status)
    {
        var list = await _vagaRepository.ListarComFiltrosAsync(usuarioId, status);
        return list.ParaRespostaJson();
    }

    public async Task<RespostavagaJson?> ObterPorIdAsync(int id)
    {
        var entidade = await _vagaRepository.ObterPorIdAsync(id);
        return entidade?.ParaRespostaJson();
    }

    public async Task<RespostavagaJson?> AtualizarAsync(int id, RequisicaoAtualizarVagaJson requisicao)
    {
        var entidade = await _vagaRepository.ObterPorIdAsync(id);
        if (entidade == null) return null;

        if (!string.IsNullOrWhiteSpace(requisicao.Titulo))
            entidade.Titulo = requisicao.Titulo;

        if (!string.IsNullOrWhiteSpace(requisicao.Descricao))
            entidade.Descricao = requisicao.Descricao;

        await _vagaRepository.AtualizarAsync(entidade);
        return entidade.ParaRespostaJson();
    }

    public async Task<bool> AtualizarStatusAsync(int id, string status)
    {
        return await _vagaRepository.AtualizarStatusAsync(id, status);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        return await _vagaRepository.DeletarAsync(id);
    }
}