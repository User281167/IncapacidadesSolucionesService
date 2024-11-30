﻿using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.auth
{
    public class AuthRoleReq
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid leaderId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido"), MaxLength(50), MinLength(3), RegularExpression("^[a-zA-Z ]+$")]
        public string Name {  get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido"), MaxLength(50), MinLength(3), RegularExpression("^[a-zA-Z ]+$")]
        public string LastName {  get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido"), MaxLength(12), MinLength(3)]
        public string Cedula {  get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido"), EmailAddress(ErrorMessage = "Formato de email no válido")]
        public string Email {  get; set; }

        public string Phone {  get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido"), MaxLength(50), MinLength(3)]
        public string Password {  get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Role { get; set; }
    }
}