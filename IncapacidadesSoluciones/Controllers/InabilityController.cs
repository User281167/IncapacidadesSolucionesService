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
    public class InabilityController : ControllerBase
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

        [HttpGet("from-user"), Authorize]
        public async Task<IActionResult> GetAllByUser([FromQuery] Guid userId)
        {
            ApiRes<List<Inability>> res;

            try
            {
                res = await InabilityService.GetAllFromUser(userId);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al procesar la solicitud");
            }

            if (!res.Success)
                return BadRequest(res.Message);

            return Ok(res);
        }

        [HttpGet("no-accepted"), Authorize(Roles = "RECEPCIONISTA")]
        public async Task<IActionResult> GetNotAccepted([FromQuery] Guid receptionistId)
        {
            try
            {
                var res = await InabilityService.GetNoAccepted(receptionistId);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<List<Inability>>
                    {
                        Message = "Error interno al procesar la solicitud" + ex.Message
                    }
                );
            }
        }
    }
}
