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
        public string Name { get; set; }

        [@Column("apellidos")]
        public string LastName { get; set; }

        [@Column("cedula")]
        public string Cedula { get; set; }

        [@Column("email")]
        public string Email { get; set; }

        [@Column("telefono")]
        public string Phone { get; set; }

        [@Column("nit_empresa")]
        public string CompanyNIT { get; set; }

        [@Column("fecha_union")]
        public DateOnly JoinDate { get; set; }

        [@Column("rol")]
        public string Role { get; set; }

        public static User FromDto(CreateUserReq dto)
        {
            return new User
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Cedula = dto.Cedula,
                Email = dto.Email,
                Phone = dto.Phone,
                CompanyNIT = dto.CompanyNIT,
                Role = dto.Role
            };
        }
    }
}
