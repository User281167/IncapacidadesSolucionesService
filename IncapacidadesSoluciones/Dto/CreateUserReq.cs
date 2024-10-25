﻿namespace IncapacidadesSoluciones.Dto
{
    public class CreateUserReq
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CompanyNIT { get; set; }
        public DateOnly JoinDate { get; set; }
        public String Role { get; set; }
    }
}
