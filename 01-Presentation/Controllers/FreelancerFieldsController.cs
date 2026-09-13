using _02_Application.Authorization;
using _02_Application.DTOs.Freelancer;
using _02_Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _01_Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class FreelancerFieldsController : ControllerBase
    {
        private readonly IFreelancerFieldsService _freelancerFieldsService;

        public FreelancerFieldsController(IFreelancerFieldsService freelancerFieldsService)
        {
            _freelancerFieldsService = freelancerFieldsService;
        }

        /// <summary>Painel de Admin - Lista todas as disponibilidades de tempo cadastradas</summary>
        /// <returns>Retorna a lista de disponibilidades de tempo ou lista vazia.</returns>
        /// <response code="200">Ok, lista de disponibilidades de tempo.</response>
        /// <response code="204">Não há disponibilidades de tempo cadastradas.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/disponibilidadesDeTempo")]
        public async Task<IActionResult> GetAllAvailableTimes()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllAvailableTimes();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Lista todas as remunerações médias cadastradas</summary>
        /// <returns>Retorna a lista de remunerações médias ou lista vazia.</returns>
        /// <response code="200">Ok, lista de remunerações médias.</response>
        /// <response code="204">Não há remunerações médias cadastradas.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/remuneracaoMedia")]
        public async Task<IActionResult> GetAllAverageRevenue()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllAverageRevenues();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Lista todos os tipos de negócio cadastrados</summary>
        /// <returns>Retorna a lista de tipos de negócio ou lista vazia.</returns>
        /// <response code="200">Ok, lista de tipos de negócio.</response>
        /// <response code="204">Não há tipos de negócio cadastrados.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/tiposDeNegocios")]
        public async Task<IActionResult> GetAllBusinessTypes()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllBusinessTypes();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Lista todos os tempos de experiência cadastrados</summary>
        /// <returns>Retorna a lista de tempos de experiência ou lista vazia.</returns>
        /// <response code="200">Ok, lista de tempos de experiência.</response>
        /// <response code="204">Não há tempos de experiência cadastrados.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/temposDeExperiencia")]
        public async Task<IActionResult> GetAllExperienceYears()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllExperienceYears();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Lista todas as preferências do freelancer cadastradas</summary>
        /// <returns>Retorna a lista de preferências do freelancer ou lista vazia.</returns>
        /// <response code="200">Ok, lista de preferências do freelancer.</response>
        /// <response code="204">Não há preferências do freelancer cadastradas.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/preferenciasDoFreelancer")]
        public async Task<IActionResult> GetAllFreelancerPreferences()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllFreelancerPreferences();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Lista todas as opções de como costuma fechar serviços cadastradas</summary>
        /// <returns>Retorna a lista de como costuma fechar serviços ou lista vazia.</returns>
        /// <response code="200">Ok, lista de como costuma fechar serviços.</response>
        /// <response code="204">Não há opções de como costuma fechar serviços cadastradas.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/comoCostumaFecharServicos")]
        public async Task<IActionResult> GetAllHowUsuallyArrangeServices()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllHowUsuallyArrangeServices();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Lista todas as máquinas que possui cadastradas</summary>
        /// <returns>Retorna a lista de máquinas que possui ou lista vazia.</returns>
        /// <response code="200">Ok, lista de máquinas que possui.</response>
        /// <response code="204">Não há máquinas que possui cadastradas.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/maquinasQuePossui")]
        public async Task<IActionResult> GetAllOwnMachines()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllOwnMachines();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Lista todas as especialidades cadastradas</summary>
        /// <returns>Retorna a lista de especialidades ou lista vazia.</returns>
        /// <response code="200">Ok, lista de especialidades.</response>
        /// <response code="204">Não há especialidades cadastradas.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/especialidades")]
        public async Task<IActionResult> GetAllSpecialties()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllSpecialties();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Lista todos os tamanhos da oficina cadastrados</summary>
        /// <returns>Retorna a lista de tamanhos da oficina ou lista vazia.</returns>
        /// <response code="200">Ok, lista de tamanhos da oficina.</response>
        /// <response code="204">Não há tamanhos da oficina cadastrados.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [HttpGet("listar/tamanhosDaOficina")]
        public async Task<IActionResult> GetAllWorkshopSizes()
        {
            ICollection<FieldsFreelancerDto> result = await _freelancerFieldsService.GetAllWorkshopSizes();

            if (result.Count < 1)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa a disponibilidade de tempo pelo ID único</summary>
        /// <param name="id">ID único do registro de disponibilidade de tempo.</param>
        /// <returns>A disponibilidade de tempo, se encontrada.</returns>
        /// <response code="200">Ok, retorna a disponibilidade de tempo.</response>
        /// <response code="404">Registro de disponibilidade de tempo não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/disponibilidadeDeTempo")]
        public async Task<IActionResult> GetAvailableTimeById(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetAvailableTimeByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa a remuneração média pelo ID único</summary>
        /// <param name="id">ID único do registro de remuneração média.</param>
        /// <returns>A remuneração média, se encontrada.</returns>
        /// <response code="200">Ok, retorna a remuneração média.</response>
        /// <response code="404">Registro de remuneração média não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/remuneracaoMedia")]
        public async Task<IActionResult> GetAverageRevenueByIdAsync(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetAverageRevenueByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa o tipo de negócio pelo ID único</summary>
        /// <param name="id">ID único do registro de tipo de negócio.</param>
        /// <returns>O tipo de negócio, se encontrado.</returns>
        /// <response code="200">Ok, retorna o tipo de negócio.</response>
        /// <response code="404">Registro de tipo de negócio não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/tipoDeNegocio")]
        public async Task<IActionResult> GetBusinessTypeByIdAsync(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetBusinessTypeByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa o tempo de experiência pelo ID único</summary>
        /// <param name="id">ID único do registro de tempo de experiência.</param>
        /// <returns>O tempo de experiência, se encontrado.</returns>
        /// <response code="200">Ok, retorna o tempo de experiência.</response>
        /// <response code="404">Registro de tempo de experiência não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/tempoDeExperiencia")]
        public async Task<IActionResult> GetExperienceYearsByIdAsync(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetExperienceYearsByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa a preferência do freelancer pelo ID único</summary>
        /// <param name="id">ID único do registro de preferência do freelancer.</param>
        /// <returns>A preferência do freelancer, se encontrada.</returns>
        /// <response code="200">Ok, retorna a preferência do freelancer.</response>
        /// <response code="404">Registro de preferência do freelancer não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/preferenciaDoFreelancer")]
        public async Task<IActionResult> GetFreelancerPreferencesByIdAsync(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetFreelancerPreferencesByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa a opção de como costuma fechar serviço pelo ID único</summary>
        /// <param name="id">ID único do registro de como costuma fechar serviço.</param>
        /// <returns>A opção de como costuma fechar serviço, se encontrada.</returns>
        /// <response code="200">Ok, retorna a opção de como costuma fechar serviço.</response>
        /// <response code="404">Registro de como costuma fechar serviço não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/comoCostumaFecharServico")]
        public async Task<IActionResult> GetHowUsuallyArrangeServicesByIdAsync(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetHowUsuallyArrangeServicesByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa a máquina que possui pelo ID único</summary>
        /// <param name="id">ID único do registro de máquina que possui.</param>
        /// <returns>A máquina que possui, se encontrada.</returns>
        /// <response code="200">Ok, retorna a máquina que possui.</response>
        /// <response code="404">Registro de máquina que possui não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/maquinaQuePossui")]
        public async Task<IActionResult> GetOwnMachineByIdAsync(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetOwnMachineByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa a especialidade pelo ID único</summary>
        /// <param name="id">ID único do registro de especialidade.</param>
        /// <returns>A especialidade, se encontrada.</returns>
        /// <response code="200">Ok, retorna a especialidade.</response>
        /// <response code="404">Registro de especialidade não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/especialidade")]
        public async Task<IActionResult> GetSpecialtyByIdAsync(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetSpecialtyByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Pesquisa o tamanho da oficina pelo ID único</summary>
        /// <param name="id">ID único do registro de tamanho da oficina.</param>
        /// <returns>O tamanho da oficina, se encontrado.</returns>
        /// <response code="200">Ok, retorna o tamanho da oficina.</response>
        /// <response code="404">Registro de tamanho da oficina não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("pesquisaPorId/tamanhoDaOficina")]
        public async Task<IActionResult> GetWorkshopSizeByIdAsync(int id)
        {
            FieldsFreelancerDto? result = await _freelancerFieldsService.GetWorkshopSizeByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Painel de Admin - Cria um novo registro de disponibilidade de tempo</summary>
        /// <param name="dto">Nome do novo registro de disponibilidade de tempo.</param>
        /// <returns>Registro de disponibilidade de tempo criado.</returns>
        /// <response code="201">Ok, registro de disponibilidade de tempo criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/disponibilidadeDeTempo")]
        public async Task<IActionResult> CreateAvailableTimeAsync(FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.CreateAvailableTimeAsync(dto))
            {
                return BadRequest();
            }
            
            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Cria um novo registro de remuneração média</summary>
        /// <param name="dto">Nome do novo registro de remuneração média.</param>
        /// <returns>Registro de remuneração média criado.</returns>
        /// <response code="201">Ok, registro de remuneração média criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/remuneracaoMedia")]
        public async Task<IActionResult> CreateAverageRevenueAsync(FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.CreateAverageRevenueAsync(dto))
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Cria um novo registro de tipo de negócio</summary>
        /// <param name="dto">Nome do novo registro de tipo de negócio.</param>
        /// <returns>Registro de tipo de negócio criado.</returns>
        /// <response code="201">Ok, registro de tipo de negócio criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/tipoDeNegocio")]
        public async Task<IActionResult> CreateBusinessTypeAsync(FieldsFreelancerDto dto)
        {           
            if (!await _freelancerFieldsService.CreateBusinessTypeAsync(dto))
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Cria um novo registro de tempo de experiência</summary>
        /// <param name="dto">Nome do novo registro de tempo de experiência.</param>
        /// <returns>Registro de tempo de experiência criado.</returns>
        /// <response code="201">Ok, registro de tempo de experiência criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/tempoDeExperiencia")]
        public async Task<IActionResult> CreateExperienceYearsAsync(FieldsFreelancerDto dto)
        {            
            if (!await _freelancerFieldsService.CreateExperienceYearsAsync(dto))
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Cria um novo registro de preferência do freelancer</summary>
        /// <param name="dto">Nome do novo registro de preferência do freelancer.</param>
        /// <returns>Registro de preferência do freelancer criado.</returns>
        /// <response code="201">Ok, registro de preferência do freelancer criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/preferenciaDoFreelancer")]
        public async Task<IActionResult> CreateFreelancerPreferencesAsync(FieldsFreelancerDto dto)
        {   
            if (!await _freelancerFieldsService.CreateFreelancerPreferencesAsync(dto))
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Cria um novo registro de como costuma fechar serviço</summary>
        /// <param name="dto">Nome do novo registro de como costuma fechar serviço.</param>
        /// <returns>Registro de como costuma fechar serviço criado.</returns>
        /// <response code="201">Ok, registro de como costuma fechar serviço criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/comoCostumaFecharServico")]
        public async Task<IActionResult> CreateHowUsuallyArrangeServicesAsync(FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.CreateHowUsuallyArrangeServicesAsync(dto))
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Cria um novo registro de máquina que possui</summary>
        /// <param name="dto">Nome do novo registro de máquina que possui.</param>
        /// <returns>Registro de máquina que possui criado.</returns>
        /// <response code="201">Ok, registro de máquina que possui criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/maquinaQuePossui")]
        public async Task<IActionResult> CreateOwnMachineAsync(FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.CreateOwnMachineAsync(dto))
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Cria um novo registro de especialidade</summary>
        /// <param name="dto">Nome do novo registro de especialidade.</param>
        /// <returns>Registro de especialidade criado.</returns>
        /// <response code="201">Ok, registro de especialidade criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/especialidade")]
        public async Task<IActionResult> CreateSpecialtyAsync(FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.CreateSpecialtyAsync(dto))
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Cria um novo registro de tamanho da oficina</summary>
        /// <param name="dto">Nome do novo registro de tamanho da oficina.</param>
        /// <returns>Registro de tamanho da oficina criado.</returns>
        /// <response code="201">Ok, registro de tamanho da oficina criado.</response>
        /// <response code="400">O nome informado já está cadastrado.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("criar/tamanhoDaOficina")]
        public async Task<IActionResult> CreateWorkshopSizeAsync(FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.CreateWorkshopSizeAsync(dto))
            {
                return BadRequest();
            }

            return CreatedAtAction(null, null);
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de disponibilidade de tempo</summary>
        /// <param name="id">ID único do registro de disponibilidade de tempo.</param>
        /// <param name="dto">Novo nome do registro de disponibilidade de tempo.</param>
        /// <returns>Registro de disponibilidade de tempo atualizado.</returns>
        /// <response code="200">Ok, registro de disponibilidade de tempo atualizado.</response>
        /// <response code="404">Registro de disponibilidade de tempo não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/disponibilidadeDeTempo")]
        public async Task<IActionResult> UpdateAvailableTimeAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateAvailableTimeAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de remuneração média</summary>
        /// <param name="id">ID único do registro de remuneração média.</param>
        /// <param name="dto">Novo nome do registro de remuneração média.</param>
        /// <returns>Registro de remuneração média atualizado.</returns>
        /// <response code="200">Ok, registro de remuneração média atualizado.</response>
        /// <response code="404">Registro de remuneração média não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/remuneracaoMedia")]
        public async Task<IActionResult> UpdateAverageRevenueAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateAverageRevenueAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de tipo de negócio</summary>
        /// <param name="id">ID único do registro de tipo de negócio.</param>
        /// <param name="dto">Novo nome do registro de tipo de negócio.</param>
        /// <returns>Registro de tipo de negócio atualizado.</returns>
        /// <response code="200">Ok, registro de tipo de negócio atualizado.</response>
        /// <response code="404">Registro de tipo de negócio não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/tipoDeNegocio")]
        public async Task<IActionResult> UpdateBusinessTypeAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateBusinessTypeAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de tempo de experiência</summary>
        /// <param name="id">ID único do registro de tempo de experiência.</param>
        /// <param name="dto">Novo nome do registro de tempo de experiência.</param>
        /// <returns>Registro de tempo de experiência atualizado.</returns>
        /// <response code="200">Ok, registro de tempo de experiência atualizado.</response>
        /// <response code="404">Registro de tempo de experiência não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/tempoDeExperiencia")]
        public async Task<IActionResult> UpdateExperienceYearsAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateExperienceYearsAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de preferência do freelancer</summary>
        /// <param name="id">ID único do registro de preferência do freelancer.</param>
        /// <param name="dto">Novo nome do registro de preferência do freelancer.</param>
        /// <returns>Registro de preferência do freelancer atualizado.</returns>
        /// <response code="200">Ok, registro de preferência do freelancer atualizado.</response>
        /// <response code="404">Registro de preferência do freelancer não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/preferenciaDoFreelancer")]
        public async Task<IActionResult> UpdateFreelancerPreferencesAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateFreelancerPreferencesAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de como costuma fechar serviço</summary>
        /// <param name="id">ID único do registro de como costuma fechar serviço.</param>
        /// <param name="dto">Novo nome do registro de como costuma fechar serviço.</param>
        /// <returns>Registro de como costuma fechar serviço atualizado.</returns>
        /// <response code="200">Ok, registro de como costuma fechar serviço atualizado.</response>
        /// <response code="404">Registro de como costuma fechar serviço não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/comoCostumaFecharServico")]
        public async Task<IActionResult> UpdateHowUsuallyArrangeServicesAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateHowUsuallyArrangeServicesAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de máquina que possui</summary>
        /// <param name="id">ID único do registro de máquina que possui.</param>
        /// <param name="dto">Novo nome do registro de máquina que possui.</param>
        /// <returns>Registro de máquina que possui atualizado.</returns>
        /// <response code="200">Ok, registro de máquina que possui atualizado.</response>
        /// <response code="404">Registro de máquina que possui não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/maquinaQuePossui")]
        public async Task<IActionResult> UpdateOwnMachineAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateOwnMachineAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de especialidade</summary>
        /// <param name="id">ID único do registro de especialidade.</param>
        /// <param name="dto">Novo nome do registro de especialidade.</param>
        /// <returns>Registro de especialidade atualizado.</returns>
        /// <response code="200">Ok, registro de especialidade atualizado.</response>
        /// <response code="404">Registro de especialidade não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/especialidade")]
        public async Task<IActionResult> UpdateSpecialtyAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateSpecialtyAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Atualiza o nome do registro de tamanho da oficina</summary>
        /// <param name="id">ID único do registro de tamanho da oficina.</param>
        /// <param name="dto">Novo nome do registro de tamanho da oficina.</param>
        /// <returns>Registro de tamanho da oficina atualizado.</returns>
        /// <response code="200">Ok, registro de tamanho da oficina atualizado.</response>
        /// <response code="404">Registro de tamanho da oficina não encontrado.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPut("atualizar/tamanhoDaOficina")]
        public async Task<IActionResult> UpdateWorkshopSizeAsync(int id, FieldsFreelancerDto dto)
        {
            if (!await _freelancerFieldsService.UpdateWorkshopSizeAsync(id, dto))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>Painel de Admin - Exclui o registro de disponibilidade de tempo</summary>
        /// <param name="id">ID único do registro de disponibilidade de tempo.</param>
        /// <returns>Registro de disponibilidade de tempo excluído.</returns>
        /// <response code="204">Ok, registro de disponibilidade de tempo excluído.</response>
        /// <response code="404">Registro de disponibilidade de tempo não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/disponibilidadeDeTempo")]
        public async Task<IActionResult> DeleteAvailableTimeAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteAvailableTimeAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }

        /// <summary>Painel de Admin - Exclui o registro de remuneração média</summary>
        /// <param name="id">ID único do registro de remuneração média.</param>
        /// <returns>Registro de remuneração média excluído.</returns>
        /// <response code="204">Ok, registro de remuneração média excluído.</response>
        /// <response code="404">Registro de remuneração média não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/remuneracaoMedia")]
        public async Task<IActionResult> DeleteAverageRevenueAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteAverageRevenueAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }

        /// <summary>Painel de Admin - Exclui o registro de tipo de negócio</summary>
        /// <param name="id">ID único do registro de tipo de negócio.</param>
        /// <returns>Registro de tipo de negócio excluído.</returns>
        /// <response code="204">Ok, registro de tipo de negócio excluído.</response>
        /// <response code="404">Registro de tipo de negócio não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/tipoDeNegocio")]
        public async Task<IActionResult> DeleteBusinessTypeAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteBusinessTypeAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }

        /// <summary>Painel de Admin - Exclui o registro de tempo de experiência</summary>
        /// <param name="id">ID único do registro de tempo de experiência.</param>
        /// <returns>Registro de tempo de experiência excluído.</returns>
        /// <response code="204">Ok, registro de tempo de experiência excluído.</response>
        /// <response code="404">Registro de tempo de experiência não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/tempoDeExperiencia")]
        public async Task<IActionResult> DeleteExperienceYearAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteExperienceYearAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }

        /// <summary>Painel de Admin - Exclui o registro de preferência do freelancer</summary>
        /// <param name="id">ID único do registro de preferência do freelancer.</param>
        /// <returns>Registro de preferência do freelancer excluído.</returns>
        /// <response code="204">Ok, registro de preferência do freelancer excluído.</response>
        /// <response code="404">Registro de preferência do freelancer não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/preferenciaDoFreelancer")]
        public async Task<IActionResult> DeleteFreelancerPreferenceAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteFreelancerPreferenceAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }

        /// <summary>Painel de Admin - Exclui o registro de como costuma fechar serviço</summary>
        /// <param name="id">ID único do registro de como costuma fechar serviço.</param>
        /// <returns>Registro de como costuma fechar serviço excluído.</returns>
        /// <response code="204">Ok, registro de como costuma fechar serviço excluído.</response>
        /// <response code="404">Registro de como costuma fechar serviço não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/comoCostumaFecharServico")]
        public async Task<IActionResult> DeleteHowUsuallyArrangeServiceAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteHowUsuallyArrangeServiceAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }

        /// <summary>Painel de Admin - Exclui o registro de máquina que possui</summary>
        /// <param name="id">ID único do registro de máquina que possui.</param>
        /// <returns>Registro de máquina que possui excluído.</returns>
        /// <response code="204">Ok, registro de máquina que possui excluído.</response>
        /// <response code="404">Registro de máquina que possui não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/maquinaQuePossui")]
        public async Task<IActionResult> DeleteOwnMachineAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteOwnMachineAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }

        /// <summary>Painel de Admin - Exclui o registro de especialidade</summary>
        /// <param name="id">ID único do registro de especialidade.</param>
        /// <returns>Registro de especialidade excluído.</returns>
        /// <response code="204">Ok, registro de especialidade excluído.</response>
        /// <response code="404">Registro de especialidade não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/especialidade")]
        public async Task<IActionResult> DeleteSpecialtyAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteSpecialtyAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }

        /// <summary>Painel de Admin - Exclui o registro de tamanho da oficina</summary>
        /// <param name="id">ID único do registro de tamanho da oficina.</param>
        /// <returns>Registro de tamanho da oficina excluído.</returns>
        /// <response code="204">Ok, registro de tamanho da oficina excluído.</response>
        /// <response code="404">Registro de tamanho da oficina não encontrado.</response>
        /// <response code="409">Não é possível deletar um campo em uso.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("deletar/tamanhoDaOficina")]
        public async Task<IActionResult> DeleteWorkshopSizeAsync(int id)
        {
            bool? result = await _freelancerFieldsService.DeleteWorkshopSizeAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Não é possível deletar um campo em uso.");
            }

            return NoContent();
        }
    }
}