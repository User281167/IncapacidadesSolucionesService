using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Services;
using IncapacidadesSoluciones.Utilities.Role;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("signup-company")]
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

        [HttpPost("signup-user")]
        public async Task<IActionResult> RegisterUser(AuthUserReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            var res = await authService.RegisterUser(req, USER_ROLE.COLLABORATOR);

            // Error creating a new user
            if (!string.IsNullOrEmpty(res.ErrorMessage) && res.User == null)
                return BadRequest(res.ErrorMessage);

            return Ok(res);
        }

        [HttpPost("login-code-update"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> CreateCode(AuthLoginCodeReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            var res = await authService.UpdateLoginCode(req);

            if (!res.Success)
                return BadRequest(res.Message);

            return Ok(res);
        }
    }
}
