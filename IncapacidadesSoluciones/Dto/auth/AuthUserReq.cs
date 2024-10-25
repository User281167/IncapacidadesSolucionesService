using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthUserReq
    {
        public Guid loginCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Cedula { get; set; }

        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
