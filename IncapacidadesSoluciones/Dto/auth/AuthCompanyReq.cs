using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthCompanyReq
    {
        [Required]
        public string Nit { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Sector { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FoundingDate { get; set; }
        [Required]
        public string LeaderName{ get; set; }
        [Required]
        public string LeaderLastName { get; set; }
        [Required]
        public string LeaderPhone { get; set; }
        [Required]
        public string LeaderEmail { get; set; }
        [Required]
        public string LeaderCedula { get; set; }
        [Required]
        public string LeaderPassword { get; set; }
    }
}
