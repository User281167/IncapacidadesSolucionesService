using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.Inability
{
    public class AddFileReq
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid UserId { get; set; }

        public Guid? InabilityId { get; set; } = default;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public IFormFile File { get; set; }
    }
}