﻿using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Models;
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
                return StatusCode(500, "Error al procesar petición. " + ex.Message);  
            }
            
            return Ok(res);
        }
    }
}
