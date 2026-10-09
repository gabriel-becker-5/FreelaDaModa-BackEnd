using _02_Application.DTOs.Vaga;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _02_Application.Mappings;
using _03_Infrastructure.Repositories.Vaga;
using _04_Domain.Enums;
using _04_Domain.Interfaces;
using DominioVaga = _04_Domain.Entities.Vaga;

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

        var entidade = new DominioVaga
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

    public async Task<IEnumerable<VagaDto>> ListarAsync(int? usuarioId, StatusVaga? status)
    {
        var vagas = await _vagaRepository.ListarNaoExcluidasAsync(usuarioId, status);
        return vagas.ParaVagaDto();
    }

    public async Task<VagaDto?> BuscarPorIdAsync(int id)
    {
        var vaga = await _vagaRepository.ObterNaoExcluidaPorIdAsync(id);
        return vaga?.ParaVagaDto();
    }

    public async Task<VagaOperacaoResult> EditarAsync(string emailUsuario, int id, UpdateVagaDto dto)
    {
        var (resultado, vaga) = await ObterVagaDoUsuarioAsync(emailUsuario, id);
        if (vaga == null) return resultado;

        // As validações de campo vazio, orçamento e prazo são feitas no controller
        if (dto.Titulo is not null)
            vaga.Titulo = dto.Titulo;

        if (dto.Descricao is not null)
            vaga.Descricao = dto.Descricao;

        if (dto.Especialidade.HasValue)
            vaga.Especialidade = dto.Especialidade.Value;

        if (dto.Modalidade.HasValue)
            vaga.Modalidade = dto.Modalidade.Value;

        if (dto.Cidade is not null)
            vaga.Cidade = dto.Cidade;

        if (dto.Estado is not null)
            vaga.Estado = dto.Estado;

        if (dto.Orcamento.HasValue)
            vaga.Orcamento = dto.Orcamento.Value;

        if (dto.PrazoConclusao.HasValue)
            vaga.PrazoConclusao = dto.PrazoConclusao.Value;

        vaga.UpdatedAt = DateTime.UtcNow;
        await _vagaRepository.AtualizarAsync(vaga);

        return new VagaOperacaoResult(VagaOperacaoStatus.Success, vaga.ParaVagaDto());
    }

    public async Task<VagaOperacaoResult> AlterarStatusAsync(string emailUsuario, int id, StatusVaga status)
    {
        var (resultado, vaga) = await ObterVagaDoUsuarioAsync(emailUsuario, id);
        if (vaga == null) return resultado;

        vaga.Status = status;
        vaga.UpdatedAt = DateTime.UtcNow;
        await _vagaRepository.AtualizarAsync(vaga);

        return new VagaOperacaoResult(VagaOperacaoStatus.Success, vaga.ParaVagaDto());
    }

    public async Task<VagaOperacaoResult> ExcluirAsync(string emailUsuario, int id)
    {
        var (resultado, vaga) = await ObterVagaDoUsuarioAsync(emailUsuario, id);
        if (vaga == null) return resultado;

        // Soft delete
        vaga.IsDeleted = true;
        vaga.UpdatedAt = DateTime.UtcNow;
        await _vagaRepository.AtualizarAsync(vaga);

        return new VagaOperacaoResult(VagaOperacaoStatus.Success);
    }

    // Valida usuário logado, existência da vaga e ownership (só o dono pode alterar)
    private async Task<(VagaOperacaoResult Resultado, DominioVaga? Vaga)> ObterVagaDoUsuarioAsync(
        string emailUsuario,
        int id)
    {
        var usuario = await _userRepository.GetUserByEmailAsync(emailUsuario);
        if (usuario == null)
        {
            return (new VagaOperacaoResult(VagaOperacaoStatus.UsuarioNaoEncontrado), null);
        }

        var vaga = await _vagaRepository.ObterNaoExcluidaPorIdAsync(id);
        if (vaga == null)
        {
            return (new VagaOperacaoResult(VagaOperacaoStatus.NaoEncontrada), null);
        }

        if (vaga.UsuarioId != usuario.Id)
        {
            return (new VagaOperacaoResult(VagaOperacaoStatus.SemPermissao), null);
        }

        return (new VagaOperacaoResult(VagaOperacaoStatus.Success), vaga);
    }
}