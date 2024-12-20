using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class DeleteAuthReq
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public Guid LeaderId { get; set; }
    }
}