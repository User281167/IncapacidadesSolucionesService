using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Dto.Company;
using IncapacidadesSoluciones.Services;
using IncapacidadesSoluciones.Utilities.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController(
        AuthService authService
    ) : ApiControllerBase
    {
        private readonly AuthService authService = authService;

        [HttpPost("signup-company")]
        public async Task<IActionResult> RegisterCompany(AuthCompanyReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            try
            {
                var res = await authService.RegisterCompany(req);

                // Error creating a new user, can fail creating company
                if (!string.IsNullOrEmpty(res.ErrorMessage) && res.User == null)
                    return BadRequest(res.ErrorMessage);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error interno al procesar la petición."
                );
            }
        }

        [HttpPut("update-company"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> UpdateCompany([FromQuery] Guid leaderId, CompanyReq req)
        {
            try
            {
                var res = await authService.UpdateCompany(leaderId, req);
                return string.IsNullOrEmpty(res) ? Ok() : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error interno al procesar la petición."
                );
            }
        }

        [HttpPost("signup-user")]
        public async Task<IActionResult> RegisterUser(AuthUserReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            try
            {
                var res = await authService.RegisterUser(req, USER_ROLE.COLLABORATOR);

                // Error creating a new user
                if (!string.IsNullOrEmpty(res.ErrorMessage) && res.User == null)
                    return BadRequest(res.ErrorMessage);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error interno al procesar la petición."
                );
            }
        }

        [HttpPost("generate-access-code"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> AddAccessCode(AuthAccessCodeReq req)
        {
            return await HandleServiceCall(
                async () => await authService.UpdateAccessCode(req)
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthLoginReq req)
        {
            if (req == null)
                return BadRequest("La información no puede ser nula.");

            try
            {
                var res = await authService.Login(req);

                if (!string.IsNullOrEmpty(res.ErrorMessage) && res.User == null)
                    return BadRequest(res.ErrorMessage);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error interno al procesar la petición."
                );
            }
        }

        [HttpGet("validate-token")]
        public IActionResult ValidateToken([FromQuery] string token)
        {
            return Ok(authService.ValidateToken(token));
        }

        [HttpPost("create-role"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> CreateRole(AuthRoleReq req)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiRes<List<string>>
                {
                    Message = "Error de validación de datos.",
                    Data = errors
                });
            }

            return await HandleServiceCall(
                async () => await authService.CreateRole(req)
            );
        }

        [HttpPut("update-role"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> UpdateRole(AuthRoleReq req)
        {
            return await HandleServiceCall(
                async () => await authService.UpdateRole(req)
            );
        }

        [HttpDelete("delete-role"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> DeleteRole(DeleteAuthReq req)
        {
            return await HandleServiceCall(
                async () => await authService.DeleteRole(req)
            );
        }
    }
}

