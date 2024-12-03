using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.UserDto
{
    public class AddNotificationReq
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid UserId { get; set; }

        public Guid? InabilityId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Message { get; set; }
    }
}