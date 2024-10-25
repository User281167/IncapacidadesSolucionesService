using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthRes
    {
        public string Token { get; set; }
        public User User { get; set; }
        public string ErrorMessage { get; set; }
    }
}
