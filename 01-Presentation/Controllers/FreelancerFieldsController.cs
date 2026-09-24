using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace _01_Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FreelancerFieldsController : ControllerBase
    {
        private readonly IFreelancerFieldsService _freelancerFieldsService;

        public FreelancerFieldsController(IFreelancerFieldsService freelancerFieldsService)
        {
            _freelancerFieldsService = freelancerFieldsService;
        }

        /// <summary>Endpoint Público para o Frontend popular menus dropdowns.</summary>
        /// <returns>Retorna todos os registros disponíveis no campo informado.</returns>
        /// <param name="fieldName">Nome do campo. Opções disponíveis: AvailableTime, ExperienceYears, OwnMachine, Specialty</param>
        /// <response code="200">Ok, retorna os registros do campo informado, se houver.</response>
        /// <response code="400">O campo informado não existe.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("listar/camposDoFreelancer")]
        public async Task<IActionResult> GetFreelancerFields(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return BadRequest("O parâmetro 'fieldName' é obrigatório.");
            }

            ICollection<IdLabelDto> result;

            switch (fieldName.ToUpper())
            {
                case "AVAILABLETIME":
                    result = await _freelancerFieldsService.GetAll<AvailableTime>();
                    break;
                case "EXPERIENCEYEARS":
                    result = await _freelancerFieldsService.GetAll<ExperienceYears>();
                    break;
                case "OWNMACHINE":
                    result = await _freelancerFieldsService.GetAll<OwnMachine>();
                    break;
                case "SPECIALTY":
                    result = await _freelancerFieldsService.GetAll<Specialty>();
                    break;
                default:
                    return BadRequest($"Campo '{fieldName}' não reconhecido.");
            }
            return Ok(result);
        }
    }
}