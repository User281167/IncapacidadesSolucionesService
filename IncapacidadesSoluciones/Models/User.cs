using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;
using IncapacidadesSoluciones.Dto;

namespace IncapacidadesSoluciones.Models
{
    [@Table("usuarios")]
    public class User: BaseModel
    {
        [@PrimaryKey("id", false)]
        public Guid Id { get; set; }
        
        [@Column("nombres")]
        public string Nombres { get; set; }

        [@Column("apellidos")]
        public string Apellidos { get; set; }

        [@Column("cedula")]
        public string Cedula { get; set; }

        [@Column("email")]
        public string Email { get; set; }

        [@Column("telefono")]
        public string Telefono { get; set; }

        [@Column("nit_empresa")]
        public string NitEmpresa { get; set; }

        [@Column("fecha_union")]
        public DateOnly FechaUnion { get; set; }

        [@Column("rol")]
        public string Rol { get; set; }

        public static User FromDto(CreateUserReq dto)
        {
            return new User
            {
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Cedula = dto.Cedula,
                Email = dto.Email,
                Telefono = dto.Telefono,
                NitEmpresa = dto.NitEmpresa,
                Rol = dto.Rol
            };
        }
    }
}
