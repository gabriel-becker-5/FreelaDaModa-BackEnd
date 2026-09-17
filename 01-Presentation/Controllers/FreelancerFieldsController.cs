using _02_Application.DTOs.Freelancer;
using _02_Application.Interfaces;
using _04_Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _01_Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class FreelancerFieldsController : ControllerBase
    {
        private readonly IFreelancerFieldsService _freelancerFieldsService;

        public FreelancerFieldsController(IFreelancerFieldsService freelancerFieldsService)
        {
            _freelancerFieldsService = freelancerFieldsService;
        }

        /// <summary>Painel de Admin - Retorna todos os registros disponíveis no campo informado</summary>
        /// <returns>Retorna todos os registros disponíveis no campo informad.</returns>
        /// <param name="fieldName">Nome do campo. Opções disponíveis: AvailableTime, AverageRevenue, BusinessType, ExperienceYears, FreelancerPreferences, HowUsuallyArrangeServices, OwnMachine, Specialty, WorkshopSize.</param>
        /// <response code="200">Ok, retorna os registros do campo informado, se houver.</response>
        /// <response code="400">O campo informado não existe.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        [HttpGet("listar/camposDoFreelancer")]
        public async Task<IActionResult> GetFreelancerFields(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return BadRequest("O parâmetro 'fieldName' é obrigatório.");
            }

            ICollection<IdLabelDto> result = [];

            switch (fieldName.ToUpper())
            {
                case "AVAILABLETIME":
                    result = await _freelancerFieldsService.GetAll<AvailableTime>();
                    break;
                case "AVERAGEREVENUE":
                    result = await _freelancerFieldsService.GetAll<AverageRevenue>();
                    break;
                case "BUSINESSTYPE":
                    result = await _freelancerFieldsService.GetAll<BusinessType>();
                    break;
                case "EXPERIENCEYEARS":
                    result = await _freelancerFieldsService.GetAll<ExperienceYears>();
                    break;
                case "FREELANCERPREFERENCES":
                    result = await _freelancerFieldsService.GetAll<FreelancerPreferences>();
                    break;
                case "HOWUSUALLYARRANGESERVICES":
                    result = await _freelancerFieldsService.GetAll<HowUsuallyArrangeServices>();
                    break;
                case "OWNMACHINE":
                    result = await _freelancerFieldsService.GetAll<OwnMachine>();
                    break;
                case "SPECIALTY":
                    result = await _freelancerFieldsService.GetAll<Specialty>();
                    break;
                case "WORKSHOPSIZE":
                    result = await _freelancerFieldsService.GetAll<WorkshopSize>();
                    break;
                default:
                    return BadRequest($"Campo '{fieldName}' não reconhecido.");
            }

            return Ok(result);
        }
    }
}