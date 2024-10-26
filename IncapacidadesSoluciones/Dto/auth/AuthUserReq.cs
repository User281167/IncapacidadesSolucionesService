using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthUserReq
    {
        public Guid LoginCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, MinLength(5), MaxLength(100)]
        public string Cedula { get; set; }

        public string Phone { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; }
    }
}
