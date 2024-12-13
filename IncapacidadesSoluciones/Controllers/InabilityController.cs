using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Services;
using IncapacidadesSoluciones.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InabilityController(
        InabilityService inabilityService
    ) : ApiControllerBase
    {
        private readonly InabilityService inabilityService = inabilityService;

        [HttpPost, Authorize(Roles = "COLABORADOR")]
        public async Task<IActionResult> AddInability(InabilityReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            return await HandleServiceCall(
                async () => await inabilityService.AddInability(req)
            );
        }

        [HttpGet("from-user"), Authorize]
        public async Task<IActionResult> GetAllByUser([FromQuery] Guid userId)
        {
            return await HandleServiceCall(
                async () => await inabilityService.GetAllFromUser(userId)
            );
        }

        [HttpGet("no-accepted"), Authorize(Roles = "RECEPCIONISTA, LIDER")]
        public async Task<IActionResult> GetNoAccepted([FromQuery] Guid receptionistId)
        {
            return await HandleServiceCall(
                async () => await inabilityService.GetNoAccepted(receptionistId)
            );
        }

        [HttpPut("accept"), Authorize(Roles = "RECEPCIONISTA, LIDER")]
        public async Task<IActionResult> AcceptInability([FromQuery] Guid id)
        {
            return await HandleServiceCall(
                async () => await inabilityService.UpdateStateInability(id, INABILITY_STATE.ACCEPTED)
            );
        }

        [HttpPut("discharge"), Authorize(Roles = "RECEPCIONISTA, LIDER")]
        public async Task<IActionResult> DischargeInability([FromQuery] Guid id)
        {
            return await HandleServiceCall(
                async () => await inabilityService.UpdateStateInability(id, INABILITY_STATE.DISCHARGED)
            );
        }

        [HttpPut("finish"), Authorize(Roles = "AUXILIAR, LIDER")]
        public async Task<IActionResult> FinishInability([FromQuery] Guid id)
        {
            return await HandleServiceCall(
                async () => await inabilityService.UpdateStateInability(id, INABILITY_STATE.FINISHED)
            );
        }

        [HttpPut("terminate"), Authorize(Roles = "AUXILIAR, LIDER")]
        public async Task<IActionResult> TerminateInability([FromQuery] Guid id)
        {
            return await HandleServiceCall(
                async () => await inabilityService.UpdateStateInability(id, INABILITY_STATE.TERMINATED)
            );
        }

        [HttpPut("advise"), Authorize(Roles = "ASESOR, LIDER")]
        public async Task<IActionResult> AdviseInability([FromQuery] Guid id, [FromQuery] bool isAdvice)
        {
            return await HandleServiceCall(
                async () => await inabilityService.UpdateAdvice(id, isAdvice)
            );
        }

        [HttpPut("payment"), Authorize(Roles = "CARTERA_JURIDICA, LIDER")]
        public async Task<IActionResult> PaymentInability(InabilityPaymentReq req)
        {
            return await HandleServiceCall(
                async () => await inabilityService.PaymentInability(req)
            );
        }

        [HttpPut("add-replacement"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> AddReplacementInability(InabilityReplacementReq req)
        {
            return await HandleServiceCall(
                async () => await inabilityService.AddReplacement(req)
            );
        }

        [HttpGet("payment-report"), Authorize(Roles = "CONTABILIDAD, LIDER")]
        public async Task<IActionResult> GetPaymentReport([FromQuery] Guid id)
        {
            return await HandleServiceCall(
                async () => await inabilityService.GetPaymentReport(id)
            );
        }

        [HttpGet("notifications"), Authorize]
        public async Task<IActionResult> GetNotifications(Guid id)
        {
            return await HandleServiceCall(
                async () => await inabilityService.GetNotifications(id)
            );
        }

        [HttpPost("add-file"), Authorize(Roles = "COLABORADOR, GESTION_DOCUMENTAL, LIDER")]
        public async Task<IActionResult> AddFile([FromForm] AddFileReq req)
        {
            return await HandleServiceCall(
                async () => await inabilityService.AddFile(req)
            );
        }

        [HttpGet("get-files"), Authorize(Roles = "COLABORADOR, GESTION_DOCUMENTAL, LIDER")]
        public async Task<IActionResult> GetFiles([FromQuery] Guid id)
        {
            return await HandleServiceCall(
                async () => await inabilityService.GetFiles(id)
            );
        }

        [HttpGet("file"), Authorize(Roles = "COLABORADOR, GESTION_DOCUMENTAL, LIDER")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            return await HandleServiceCall(
                async () => await inabilityService.GetFile(fileName)
            );
        }
    }
}
