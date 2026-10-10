using _02_Application.DTOs.Dashboard;
using _03_Infrastructure.Repositories.Vaga;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _02_Application.Services.Dashboard
{
    public class ObterDashboardFreelancerUseCase
    {
        private const int QuantidadeVagasRecomendadas = 2;
        private const int QuantidadeConversasRecentes = 3;

        private readonly IUserRepository _userRepository;
        private readonly IVagaRepository _vagaRepository;
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly IAvaliacaoRepository _avaliacaoRepository;
        private readonly IMensagemRepository _mensagemRepository;

        public ObterDashboardFreelancerUseCase(
            IUserRepository userRepository,
            IVagaRepository vagaRepository,
            IOrdemServicoRepository ordemServicoRepository,
            ICandidaturaRepository candidaturaRepository,
            IAvaliacaoRepository avaliacaoRepository,
            IMensagemRepository mensagemRepository)
        {
            _userRepository = userRepository;
            _vagaRepository = vagaRepository;
            _ordemServicoRepository = ordemServicoRepository;
            _candidaturaRepository = candidaturaRepository;
            _avaliacaoRepository = avaliacaoRepository;
            _mensagemRepository = mensagemRepository;
        }

        public async Task<FreelancerDashboardResponseDto> ExecutarAsync(int freelancerId)
        {
            var usuario = await _userRepository.GetUserByIdAsync(freelancerId);
            string nomeExibicao = usuario?.LegalResponsibleFullName ?? string.Empty;

            (double? media, int totalAvaliacoes) = await _avaliacaoRepository.GetMediaAvaliacoesRecebidasAsync(freelancerId);

            var vagasAbertas = await _vagaRepository.ListarAbertasRecentesAsync(QuantidadeVagasRecomendadas);
            var vagasRecomendadas = new List<VagaResumoDto>();
            foreach (var vaga in vagasAbertas)
            {
                vagasRecomendadas.Add(new VagaResumoDto(
                    vaga.Id,
                    vaga.Titulo,
                    vaga.Status.ToString(),
                    vaga.Orcamento,
                    vaga.DataPublicacao,
                    vaga.UsuarioId,
                    await DashboardHelpers.ResolverNomeEmpresaAsync(_userRepository, vaga.UsuarioId)));
            }

            var ordensServico = await _ordemServicoRepository.ListByFreelancerIdAsync(freelancerId);
            var producoesAtivas = ordensServico.Where(DashboardHelpers.EstaEmAndamento).ToList();
            var producoesConcluidas = ordensServico.Where(DashboardHelpers.EstaConcluida).ToList();
            decimal faturamentoTotal = producoesConcluidas.Sum(os => os.Valor);

            var producoes = new List<OrdemServicoResumoDto>();
            foreach (var os in producoesAtivas)
            {
                producoes.Add(new OrdemServicoResumoDto(
                    os.Id,
                    os.Titulo,
                    os.Status ?? StatusOrdemServico.EmAndamento.ParaTexto(),
                    os.Categoria,
                    os.Modalidade,
                    os.Cidade,
                    os.Valor,
                    os.Prazo,
                    os.FreelancerId,
                    nomeExibicao,
                    os.UserId,
                    await DashboardHelpers.ResolverNomeEmpresaAsync(_userRepository, os.UserId)));
            }

            var candidaturas = await _candidaturaRepository.ListByFreelancerIdAsync(freelancerId);

            var mensagens = await _mensagemRepository.ListByParticipanteAsync(freelancerId);
            var conversasRecentes = await DashboardHelpers.MontarPreviewConversasAsync(
                _userRepository, freelancerId, mensagens, QuantidadeConversasRecentes);

            return new FreelancerDashboardResponseDto(
                nomeExibicao,
                media.HasValue ? (decimal)media.Value : null,
                totalAvaliacoes,
                vagasRecomendadas,
                producoesAtivas.Count,
                candidaturas.Count,
                faturamentoTotal,
                producoes,
                conversasRecentes);
        }
    }
}
