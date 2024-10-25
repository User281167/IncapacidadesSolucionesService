﻿using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto.Company
{
    public class CompanyReq
    {
        public Guid Id { get; set; }

        [Required]
        public string Nit { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        public DateOnly CreatedAt { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        [Required]
        public string Type { get; set; }
        
        [Required]
        public string Sector { get; set; }
    }
}
