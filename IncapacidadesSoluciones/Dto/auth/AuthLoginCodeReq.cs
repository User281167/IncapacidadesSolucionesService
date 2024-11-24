using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthLoginCodeReq
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El id de la empresa es requerido")]
        public Guid CompanyId { get; set; }
        
        public DateOnly ExpirationDate { get; set; }
    }
}
