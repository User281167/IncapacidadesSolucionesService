using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.Inability
{
    public class InabilityPaymentReq
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid LeaderId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public ulong CompanyPayment { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public ulong HealthEntityPayment { get; set; }
    }
}