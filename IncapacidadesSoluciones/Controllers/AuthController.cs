using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Dto.Company;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterCompany(AuthCompanyReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            var res = await authService.RegisterCompany(req);

            // Error creating a new user, can fail creating company
            if (!string.IsNullOrEmpty(res.ErrorMessage) && res.User == null)
                return BadRequest(res.ErrorMessage);

            return Ok(res);
        }
    }
}
