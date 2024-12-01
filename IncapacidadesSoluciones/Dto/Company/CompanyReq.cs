using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.Company
{
    public class CompanyReq
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nit de la empresa es requerido"), MinLength(1)]
        public string Nit { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es requerido"), MinLength(1)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "El email de la empresa es requerido"), EmailAddress]
        public string Email { get; set; }

        public DateOnly? Founded { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "El tipo de la empresa es requerido"), MinLength(1)]
        public string Type { get; set; }

        [Required(ErrorMessage = "El sector de la empresa es requerido"), MinLength(1)]
        public string Sector { get; set; }
    }
}
