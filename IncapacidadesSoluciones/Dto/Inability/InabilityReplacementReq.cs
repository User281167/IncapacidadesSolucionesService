using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.Inability
{
    public class InabilityReplacementReq
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid InabilityId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid LeaderId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid ReplacementId { get; set; }
    }
}