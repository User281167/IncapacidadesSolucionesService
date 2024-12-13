using IncapacidadesSoluciones.Dto;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        public async Task<IActionResult> HandleServiceCall<T>(Func<Task<ApiRes<T>>> serviceCall)
        {
            try
            {
                var res = await serviceCall();
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<T>
                    {
                        Message = "Error interno al procesar la solicitud."
                    }
                );
            }
        }
    }
}
