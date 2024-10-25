using IncapacidadesSoluciones.Models;
using Microsoft.AspNetCore.Mvc;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthRes
    {
        public string Token { get; set; }
        public User UserData { get; set; }
        public string ErrorMessage { get; set; }
    }
}
