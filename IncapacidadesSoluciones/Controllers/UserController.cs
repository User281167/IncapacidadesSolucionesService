using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpPut("update"), Authorize]
        public async Task<IActionResult> UpdateUserInfo(UserReq user)
        {
            if (user == null)
                return BadRequest("La información no puede ser nula.");

            ApiRes<User> res;

            try
            {
                res = await userService.UpdateUser(user);

                if (!res.Success)
                    return BadRequest(res.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar petición.");
            }

            return Ok(res);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetUserInfo(Guid userId, Guid searchBy)
        {
            try
            {
                var res = await userService.GetUserInfo(userId, searchBy);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<User>
                    {
                        Message = "Error interno al procesar la solicitud. " + ex.Message
                    }
                );
            }
        }

        [HttpGet("search-user"), Authorize]
        public async Task<IActionResult> SearchUser(Guid searchBy, string? name, string? lastName, string? cedula)
        {
            try
            {
                var res = await userService.SearchUser(searchBy, name, lastName, cedula);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<User>
                    {
                        Message = "Error interno al procesar la solicitud."
                    }
                );
            }
        }

        [HttpGet("search-collaborator"), Authorize]
        public async Task<IActionResult> SearchCollaborator(Guid searchBy, string? name, string? lastName, string? cedula)
        {
            try
            {
                var res = await userService.SearchCollaborator(searchBy, name, lastName, cedula);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<List<UserInfoRes>>
                    {
                        Message = "Error interno al procesar la solicitud. " + ex.Message
                    }
                );
            }
        }

        [HttpGet("special-roles"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> GetSpecialRoles(Guid leaderId)
        {
            try
            {
                ApiRes<List<User>> res = await userService.GetSpecialRoles(leaderId);
                return res.Success ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiRes<List<User>>
                    {
                        Message = "Error interno al procesar la solicitud."
                    }
                );
            }
        }
    }
}
