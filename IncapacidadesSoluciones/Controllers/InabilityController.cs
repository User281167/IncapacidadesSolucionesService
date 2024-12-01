using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Services;
using IncapacidadesSoluciones.Utilities;
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
        public async Task<IActionResult> GetNoAccepted([FromQuery] Guid receptionistId)
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

        [HttpPut("accept"), Authorize(Roles = "RECEPCIONISTA")]
        public async Task<IActionResult> AcceptInability([FromQuery] Guid id)
        {
            try
            {
                var res = await InabilityService.UpdateStateInability(id, INABILITY_STATE.ACCEPTED);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud" + ex.Message
                    }
                );
            }
        }

        [HttpPut("discharge"), Authorize(Roles = "RECEPCIONISTA")]
        public async Task<IActionResult> DischargeInability([FromQuery] Guid id)
        {
            try
            {
                var res = await InabilityService.UpdateStateInability(id, INABILITY_STATE.DISCHARGED);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud" + ex.Message
                    }
                );
            }
        }

        [HttpPut("finish"), Authorize(Roles = "AUXILIAR")]
        public async Task<IActionResult> FinishInability([FromQuery] Guid id)
        {
            try
            {
                var res = await InabilityService.UpdateStateInability(id, INABILITY_STATE.FINISHED);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud" + ex.Message
                    }
                );
            }
        }

        [HttpPut("terminate"), Authorize(Roles = "AUXILIAR")]
        public async Task<IActionResult> TerminateInability([FromQuery] Guid id)
        {
            try
            {
                var res = await InabilityService.UpdateStateInability(id, INABILITY_STATE.TERMINATED);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud" + ex.Message
                    }
                );
            }
        }

        [HttpPut("advise"), Authorize(Roles = "ASESOR")]
        public async Task<IActionResult> AdviseInability([FromQuery] Guid id, [FromQuery] bool isAdvice)
        {
            try
            {
                var res = await InabilityService.UpdateAdvice(id, isAdvice);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud" + ex.Message
                    }
                );
            }
        }
    }
}
