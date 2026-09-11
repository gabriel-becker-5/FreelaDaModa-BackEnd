using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using _02_Application.DTOs.Vaga;
using _02_Application.Services.Vaga;
using _04_Domain.Interfaces;

namespace _01_Presentation.Controllers
{
    [ApiController]
    [Route("api/v{version}/[controller]")]
    public class VagasController : ControllerBase
    {
        private readonly IVagaRepository _vagaRepository;

        public VagasController(IVagaRepository vagaRepository)
        {
            _vagaRepository = vagaRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RequisicaoRegistrarVagaJson requisicao)
        {
            var useCase = new RegistrarVagaUseCase(_vagaRepository);
            var resposta = await useCase.Executar(requisicao);

            return StatusCode(201, resposta);
        }
    }
}