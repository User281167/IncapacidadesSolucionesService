using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(
        UserService userService
    ) : ApiControllerBase
    {
        private readonly UserService userService = userService;

        [HttpPut("update"), Authorize]
        public async Task<IActionResult> UpdateUserInfo(UserReq user)
        {
            if (user == null)
                return BadRequest("La información no puede ser nula.");

            return await HandleServiceCall(
                async () => await userService.UpdateUser(user)
            );
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetUserInfo(Guid userId, Guid searchBy)
        {
            return await HandleServiceCall(
                async () => await userService.GetUserInfo(userId, searchBy)
            );
        }

        [HttpGet("search-user"), Authorize]
        public async Task<IActionResult> SearchUser(Guid searchBy, string? name, string? lastName, string? cedula)
        {
            return await HandleServiceCall(
                async () => await userService.SearchUser(searchBy, name, lastName, cedula)
            );
        }

        [HttpGet("search-collaborator"), Authorize]
        public async Task<IActionResult> SearchCollaborator(Guid searchBy, string? name, string? lastName, string? cedula)
        {
            return await HandleServiceCall(
                async () => await userService.SearchCollaborator(searchBy, name, lastName, cedula)
            );
        }

        [HttpGet("special-roles"), Authorize(Roles = "LIDER")]
        public async Task<IActionResult> GetSpecialRoles(Guid leaderId)
        {
            return await HandleServiceCall(
                async () => await userService.GetSpecialRoles(leaderId)
            );
        }

        [HttpGet("notifications"), Authorize]
        public async Task<IActionResult> GetNotifications(Guid userId)
        {
            return await HandleServiceCall(
                async () => await userService.GetNotifications(userId)
            );
        }

        [HttpPost("notifications"), Authorize]
        public async Task<IActionResult> AddNotification(AddNotificationReq req)
        {
            return await HandleServiceCall(
                async () => await userService.AddNotification(req)
            );
        }
    }
}
