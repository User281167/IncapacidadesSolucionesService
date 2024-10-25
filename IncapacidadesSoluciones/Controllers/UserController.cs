using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    [ApiController]
    [Route("user/[controller]")]
    public class UserController: ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(CreateUserReq newUser, Supabase.Client client)
        {
            if (newUser == null)
                return BadRequest("El usuario no puede ser nulo");

            var createdUser = await userService.CreateUser(newUser, client);

            if (createdUser == null)
                return Problem("El usuario no ser pudo crear");

            return Ok(createdUser);
        }
    }
}
