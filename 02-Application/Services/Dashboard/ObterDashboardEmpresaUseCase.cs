using _02_Application.DTOs.Dashboard;
using _03_Infrastructure.Repositories.Vaga;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Services.Dashboard
{
    public class ObterDashboardEmpresaUseCase
    {
        private const int QuantidadeCandidaturasRecentes = 5;
        private const int QuantidadeConversasRecentes = 3;

        private readonly IUserRepository _userRepository;
        private readonly IVagaRepository _vagaRepository;
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly IMensagemRepository _mensagemRepository;

        public ObterDashboardEmpresaUseCase(
            IUserRepository userRepository,
            IVagaRepository vagaRepository,
            IOrdemServicoRepository ordemServicoRepository,
            ICandidaturaRepository candidaturaRepository,
            IMensagemRepository mensagemRepository)
        {
            _userRepository = userRepository;
            _vagaRepository = vagaRepository;
            _ordemServicoRepository = ordemServicoRepository;
            _candidaturaRepository = candidaturaRepository;
            _mensagemRepository = mensagemRepository;
        }

        public async Task<CompanyDashboardResponseDto> ExecutarAsync(int empresaUserId)
        {
            string nomeExibicao = await DashboardHelpers.ResolverNomeEmpresaAsync(_userRepository, empresaUserId);

            var ordensServico = await _ordemServicoRepository.ListByEmpresaIdAsync(empresaUserId);
            var ordensAtivas = ordensServico.Where(DashboardHelpers.EstaEmAndamento).ToList();
            decimal investimentoTotal = ordensServico.Sum(os => os.Valor);

            var osAtivas = new List<OrdemServicoResumoDto>();
            foreach (var os in ordensAtivas)
            {
                osAtivas.Add(new OrdemServicoResumoDto(
                    os.Id,
                    os.Titulo,
                    os.Status ?? StatusOrdemServico.EmAndamento.ParaTexto(),
                    os.Categoria,
                    os.Modalidade,
                    os.Cidade,
                    os.Valor,
                    os.Prazo,
                    os.FreelancerId,
                    os.FreelancerId.HasValue
                        ? await DashboardHelpers.ResolverNomeFreelancerAsync(_userRepository, os.FreelancerId.Value)
                        : null,
                    empresaUserId,
                    nomeExibicao));
            }

            var vagas = await _vagaRepository.ListarPorEmpresaAsync(empresaUserId);
            int vagasAbertas = vagas.Count(v => v.Status == StatusVaga.Aberta || v.Status == StatusVaga.Pausada);
            var minhasVagas = vagas
                .Where(v => v.Status != StatusVaga.Encerrada)
                .Select(v => new VagaResumoDto(
                    v.Id,
                    v.Titulo,
                    v.Status.ToString(),
                    v.Orcamento,
                    v.DataPublicacao,
                    empresaUserId,
                    nomeExibicao))
                .ToList();

            var candidaturas = await _candidaturaRepository.ListByEmpresaIdAsync(empresaUserId);

            var candidaturasRecentes = new List<CandidaturaResumoDto>();
            foreach (var candidatura in candidaturas.Take(QuantidadeCandidaturasRecentes))
            {
                candidaturasRecentes.Add(new CandidaturaResumoDto(
                    candidatura.Id,
                    candidatura.VagaId,
                    candidatura.Vaga?.Titulo ?? "Vaga",
                    candidatura.FreelancerId,
                    await DashboardHelpers.ResolverNomeFreelancerAsync(_userRepository, candidatura.FreelancerId),
                    DashboardHelpers.MapStatusCandidatura(candidatura.Status),
                    candidatura.DataCandidatura));
            }

            var mensagens = await _mensagemRepository.ListByParticipanteAsync(empresaUserId);
            var conversasRecentes = await DashboardHelpers.MontarPreviewConversasAsync(
                _userRepository, empresaUserId, mensagens, QuantidadeConversasRecentes);

            return new CompanyDashboardResponseDto(
                nomeExibicao,
                osAtivas,
                minhasVagas,
                vagasAbertas,
                candidaturas.Count,
                ordensAtivas.Count,
                investimentoTotal,
                candidaturasRecentes,
                conversasRecentes);
        }
    }
}
