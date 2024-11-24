using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthUserReq
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string LoginCode { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido y debe comenzar con una letra"), MinLength(3), MaxLength(50), RegularExpression(@"^[a-zA-Z]*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido y debe comenzar con una letra"), MinLength(3), MaxLength(50), RegularExpression(@"^[a-zA-Z]*")]
        public string LastName { get; set; }

        [Required, MinLength(5), MaxLength(100)]
        public string Cedula { get; set; }

        public string Phone { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido y debe ser un correo valido"), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido y debe tener al menos 6 caracteres"), MinLength(6), MaxLength(100)]
        public string Password { get; set; }
    }
}
