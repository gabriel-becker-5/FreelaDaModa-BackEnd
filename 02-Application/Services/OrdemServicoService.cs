using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities;
using _04_Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_Application.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IUserService _userService;

        public OrdemServicoService(
        IOrdemServicoRepository ordemServicoRepository,
        IUserService userService)
        {
            _ordemServicoRepository = ordemServicoRepository;
            _userService = userService;
        }

        public async Task<List<OrdemServico>> ListAllAsync()
        {
            return await _ordemServicoRepository.ListAllAsync();
        }

        public async Task<OrdemServico?> GetByIdAsync(int id)
        {
            return await _ordemServicoRepository.GetByIdAsync(id);
        }

        public async Task<OrdemServico> CreateAsync(OrdemServicoDto dto)
        {
            OrdemServico ordemServico = new()
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Categoria = dto.Categoria,
                Modalidade = dto.Modalidade,
                Cidade = dto.Cidade,
                Valor = dto.Valor,
                Prazo = dto.Prazo,
                Status = dto.Status,
                Observacoes = dto.Observacoes,
                FreelancerId = dto.FreelancerId
            };

            return await _ordemServicoRepository.CreateAsync(ordemServico);
        }

        public async Task<bool> UpdateAsync(OrdemServicoDto dto)
        {
            OrdemServico? ordemServico =
                await _ordemServicoRepository.GetByIdAsync(dto.Id);

            if (ordemServico == null)
            {
                return false;
            }

            ordemServico.Titulo = dto.Titulo;
            ordemServico.Descricao = dto.Descricao;
            ordemServico.Categoria = dto.Categoria;
            ordemServico.Modalidade = dto.Modalidade;
            ordemServico.Cidade = dto.Cidade;
            ordemServico.Valor = dto.Valor;
            ordemServico.Prazo = dto.Prazo;
            ordemServico.Status = dto.Status;
            ordemServico.Observacoes = dto.Observacoes;
            ordemServico.FreelancerId = dto.FreelancerId;

            await _ordemServicoRepository.UpdateAsync(ordemServico);

            return true;
        }
    }
}
