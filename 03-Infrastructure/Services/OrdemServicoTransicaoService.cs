using _02_Application.DTOs.OrdemServico;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _03_Infrastructure.Data;
using _04_Domain.Entities;
using _04_Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace _03_Infrastructure.Services
{
    // Concluir/cancelar a OS (#18): transição atômica de "Em andamento" para um estado final,
    // usando o mesmo padrão de ExecuteUpdateAsync condicional do #16 para evitar corrida
    // entre concluir e cancelar disparados ao mesmo tempo.
    public class OrdemServicoTransicaoService : IOrdemServicoTransicaoService
    {
        private readonly AppDbContext _context;

        public OrdemServicoTransicaoService(AppDbContext context)
        {
            _context = context;
        }

        public Task<OrdemServicoTransicaoResultadoDto> ConcluirAsync(int ordemServicoId, int usuarioLogadoId)
        {
            return TransicionarAsync(
                ordemServicoId,
                usuarioLogadoId,
                somenteEmpresaDona: true,
                statusDestino: StatusOrdemServico.Concluida,
                mensagemSucesso: "Ordem de serviço concluída com sucesso.");
        }

        public Task<OrdemServicoTransicaoResultadoDto> CancelarAsync(int ordemServicoId, int usuarioLogadoId)
        {
            return TransicionarAsync(
                ordemServicoId,
                usuarioLogadoId,
                somenteEmpresaDona: false,
                statusDestino: StatusOrdemServico.Cancelada,
                mensagemSucesso: "Ordem de serviço cancelada com sucesso.");
        }

        private async Task<OrdemServicoTransicaoResultadoDto> TransicionarAsync(
            int ordemServicoId,
            int usuarioLogadoId,
            bool somenteEmpresaDona,
            StatusOrdemServico statusDestino,
            string mensagemSucesso)
        {
            OrdemServico? ordemServico = await _context.OrdensServico
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == ordemServicoId && !o.IsDeleted);

            if (ordemServico == null)
            {
                return NaoEncontrada();
            }

            bool ehEmpresaDona = ordemServico.UserId == usuarioLogadoId;
            bool ehFreelancerDaOs = ordemServico.FreelancerId == usuarioLogadoId;
            bool autorizado = somenteEmpresaDona ? ehEmpresaDona : (ehEmpresaDona || ehFreelancerDaOs);

            // Quem não participa da OS nunca descobre o estado dela: 403 antes de checar o status.
            if (!autorizado)
            {
                return new OrdemServicoTransicaoResultadoDto(
                    OrdemServicoTransicaoResultado.NaoAutorizado,
                    "Você não tem permissão para realizar esta operação nesta ordem de serviço.",
                    null);
            }

            if (!EstaEmAndamento(ordemServico.Status))
            {
                return EstadoInvalido();
            }

            // Reivindicação atômica: só segue quem conseguir mudar o status nesta UPDATE condicional,
            // evitando que concluir e cancelar simultâneos apliquem os dois efeitos na mesma OS.
            // Compara com o valor bruto lido (statusBruto), não com o texto canônico: uma OS legada
            // gravada como "EM ANDAMENTO" ou "Em andamento " já passou na checagem tolerante acima
            // (EstaEmAndamento/TryParse) e não pode falhar aqui só por diferir na grafia exata.
            string statusBruto = ordemServico.Status!;

            int linhasAfetadas = await _context.OrdensServico
                .Where(o => o.Id == ordemServicoId
                    && !o.IsDeleted
                    && o.Status == statusBruto)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(o => o.Status, statusDestino.ParaTexto()));

            if (linhasAfetadas == 0)
            {
                OrdemServico? atual = await _context.OrdensServico
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == ordemServicoId);

                if (atual == null || atual.IsDeleted)
                {
                    return NaoEncontrada();
                }

                return EstadoInvalido();
            }

            return new OrdemServicoTransicaoResultadoDto(
                OrdemServicoTransicaoResultado.Sucesso,
                mensagemSucesso,
                ordemServicoId);
        }

        private static bool EstaEmAndamento(string? status)
        {
            return StatusOrdemServicoExtensions.TryParse(status, out StatusOrdemServico statusAtual)
                && statusAtual == StatusOrdemServico.EmAndamento;
        }

        private static OrdemServicoTransicaoResultadoDto NaoEncontrada()
        {
            return new OrdemServicoTransicaoResultadoDto(
                OrdemServicoTransicaoResultado.NaoEncontrada,
                "Ordem de serviço não encontrada.",
                null);
        }

        private static OrdemServicoTransicaoResultadoDto EstadoInvalido()
        {
            return new OrdemServicoTransicaoResultadoDto(
                OrdemServicoTransicaoResultado.EstadoInvalido,
                "Esta ordem de serviço não está \"Em andamento\" e não pode ser concluída ou cancelada.",
                null);
        }
    }
}
