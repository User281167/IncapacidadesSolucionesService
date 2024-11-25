using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.UserDto
{
    public class UserReq
    {
        [Required(ErrorMessage = "El id es requerido")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido"), MaxLength(50), MinLength(3), RegularExpression(@"^[a-zA-Z]*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido"), MaxLength(50), MinLength(3), RegularExpression(@"^[a-zA-Z]*")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido"), MaxLength(20), MinLength(5)]
        public string Cedula { get; set; }

        public string Phone { get; set; }
    }
}
