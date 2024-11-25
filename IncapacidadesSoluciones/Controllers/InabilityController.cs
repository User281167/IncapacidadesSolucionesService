using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InabilityController: ControllerBase
    {
        private readonly InabilityService InabilityService;

        public InabilityController(InabilityService InabilityService)
        {
            this.InabilityService = InabilityService;
        }

        [HttpPost, Authorize(Roles = "COLABORADOR")]
        public async Task<IActionResult> AddInability(InabilityReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            var res = await InabilityService.AddInability(req);

            if (!res.Success)
                return BadRequest(res.Message);

            return Ok(res);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetAllByUser([FromQuery] Guid id)
        {
            ApiRes<List<Inability>> res;

            try
            {
                res = await InabilityService.GetAllFromUser(id);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al procesar la solicitud");
            }

            if (!res.Success)
                return BadRequest(res.Message);

            return Ok(res);
        }
    }
}
