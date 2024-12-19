using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthCompanyReq
    {
        [Required(ErrorMessage = "El nit de la empresa es requerido"), MinLength(1)]
        public string Nit { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es requerido"), MinLength(1)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "El email de la empresa es requerido"), EmailAddress]
        public string Email { get; set; }

        public DateOnly? Founded { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "El tipo de la empresa es requerido"), MinLength(1)]
        public string Type { get; set; }

        [Required(ErrorMessage = "El sector de la empresa es requerido"), MinLength(1)]
        public string Sector { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido y debe comenzar con una letra"), MinLength(3), MaxLength(50)]
        public string LeaderName { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido y debe comenzar con una letra"), MinLength(3), MaxLength(50)]
        public string LeaderLastName { get; set; }

        [Required, MinLength(5), MaxLength(12)]
        public string LeaderCedula { get; set; }

        public string? LeaderPhone { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido y debe ser un correo valido"), EmailAddress]
        public string LeaderEmail { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido y debe tener al menos 6 caracteres"), MinLength(6), MaxLength(30)]
        public string Password { get; set; }
    }
}
