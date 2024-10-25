namespace IncapacidadesSoluciones.Dto
{
    public class CreateUserReq
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string NitEmpresa { get; set; }
        public DateOnly FechaUnion { get; set; }
        public String Rol { get; set; }
    }
}
