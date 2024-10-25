using IncapacidadesSoluciones.Dto.Company;
using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthCompanyReq
    {
        [Required]
        public CompanyReq Company { get; set; }

        [Required]
        public AuthUserReq User { get; set; }
    }
}
