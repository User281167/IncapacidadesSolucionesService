using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.Inability
{
    public class InabilityReq
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid IdCollaborator { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Type { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string HealthEntity { get; set; }
    }
}
