using _02_Application.Interfaces;
using _02_Application.DTOs;
using _04_Domain.Entities;
using _04_Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_Application.Services
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;

        public AvaliacaoService(IAvaliacaoRepository avaliacaoRepository)
        {
            _avaliacaoRepository = avaliacaoRepository;
        }

        public async Task<Avaliacao> CreateAsync(AvaliacaoDto dto)
        {
            Avaliacao avaliacao = new()
            {
                OrdemServicoId = dto.OrdemServicoId,
                Nota = dto.Nota,
                Comentario = dto.Comentario
            };

            return await _avaliacaoRepository.CreateAsync(avaliacao);
        }

        public async Task<List<Avaliacao>> ListByOrdemServicoAsync(int ordemServicoId)
        {
            return await _avaliacaoRepository
                .ListByOrdemServicoAsync(ordemServicoId);
        }
    }
}
