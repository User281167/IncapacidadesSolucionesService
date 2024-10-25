﻿using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController: ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpPost, Authorize(Roles="LIDER")]
        public async Task<IActionResult> PostUser(CreateUserReq newUser, Supabase.Client client)
        {
            if (newUser == null)
                return BadRequest("El usuario no puede ser nulo");

            var createdUser = await userService.CreateUser(newUser, client);

            if (createdUser == null)
                return Problem("El usuario no pudo ser creado");

            return Ok(createdUser);
        }
    }
}
