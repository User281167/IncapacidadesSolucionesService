using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.auth;
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
        private readonly InabilityService inabilityService;

        public InabilityController(InabilityService inabilityService)
        {
            this.inabilityService = inabilityService;
        }

        [HttpPost, Authorize(Roles = "COLABORADOR")]
        public async Task<IActionResult> AddInability(InabilityReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            var res = await inabilityService.AddInability(req);

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
                res = await inabilityService.GetAllFromUser(userId);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al procesar la solicitud");
            }

            if (!res.Success)
                return BadRequest(res.Message);

            return Ok(res);
        }

        [HttpGet("no-accepted"), Authorize(Roles = "RECEPCIONISTA, LIDER")]
        public async Task<IActionResult> GetNoAccepted([FromQuery] Guid receptionistId)
        {
            try
            {
                var res = await inabilityService.GetNoAccepted(receptionistId);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<List<Inability>>
                    {
                        Message = "Error interno al procesar la solicitud"
                    }
                );
            }
        }

        [HttpPut("accept"), Authorize(Roles = "RECEPCIONISTA, LIDER")]
        public async Task<IActionResult> AcceptInability([FromQuery] Guid id)
        {
            try
            {
                var res = await inabilityService.UpdateStateInability(id, INABILITY_STATE.ACCEPTED);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud"
                    }
                );
            }
        }

        [HttpPut("discharge"), Authorize(Roles = "RECEPCIONISTA, LIDER")]
        public async Task<IActionResult> DischargeInability([FromQuery] Guid id)
        {
            try
            {
                var res = await inabilityService.UpdateStateInability(id, INABILITY_STATE.DISCHARGED);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud"
                    }
                );
            }
        }

        [HttpPut("finish"), Authorize(Roles = "AUXILIAR, LIDER")]
        public async Task<IActionResult> FinishInability([FromQuery] Guid id)
        {
            try
            {
                var res = await inabilityService.UpdateStateInability(id, INABILITY_STATE.FINISHED);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud"
                    }
                );
            }
        }

        [HttpPut("terminate"), Authorize(Roles = "AUXILIAR, LIDER")]
        public async Task<IActionResult> TerminateInability([FromQuery] Guid id)
        {
            try
            {
                var res = await inabilityService.UpdateStateInability(id, INABILITY_STATE.TERMINATED);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud"
                    }
                );
            }
        }

        [HttpPut("advise"), Authorize(Roles = "ASESOR, LIDER")]
        public async Task<IActionResult> AdviseInability([FromQuery] Guid id, [FromQuery] bool isAdvice)
        {
            try
            {
                var res = await inabilityService.UpdateAdvice(id, isAdvice);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud"
                    }
                );
            }
        }

        [HttpPut("payment"), Authorize(Roles = "CARTERA_JURIDICA, LIDER")]
        public async Task<IActionResult> PaymentInability(InabilityPaymentReq req)
        {
            try
            {
                var res = await inabilityService.PaymentInability(req);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud"
                    }
                );
            }
        }

        [HttpPut("add-replacement"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> AddReplacementInability(InabilityReplacementReq req)
        {
            try
            {
                var res = await inabilityService.AddReplacement(req);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<Inability>
                    {
                        Message = "Error interno al procesar la solicitud"
                    }
                );
            }
        }

        [HttpGet("payment-report"), Authorize(Roles = "CONTABILIDAD, LIDER")]
        public async Task<IActionResult> GetPaymentReport([FromQuery] Guid id)
        {
            try
            {
                var res = await inabilityService.GetPaymentReport(id);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<List<Inability>>
                    {
                        Message = "Error interno al procesar la solicitud."
                    }
                );
            }
        }

        [HttpGet("notifications"), Authorize]
        public async Task<IActionResult> GetNotifications(Guid id)
        {
            var res = await inabilityService.GetNotifications(id);

            if (res.Success)
                return Ok(res.Data);
            else
                return StatusCode(
                    500,
                    new ApiRes<List<Notification>>
                    {
                        Message = "Error interno al procesar la solicitud."
                    }
                );
        }
    }
}
