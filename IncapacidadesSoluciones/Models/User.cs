using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;
using IncapacidadesSoluciones.Dto.auth;

namespace IncapacidadesSoluciones.Models
{
    [@Table("users")]
    public class User : BaseModel
    {
        [@PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [@Column("name")]
        public string Name { get; set; } = string.Empty;

        [@Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [@Column("cedula")]
        public string Cedula { get; set; } = string.Empty;

        [@Column("email")]
        public string Email { get; set; } = string.Empty;

        [@Column("phone")]
        public string? Phone { get; set; }

        [@Column("company_nit")]
        public string CompanyNIT { get; set; } = string.Empty;

        [@Column("join_date")]
        public DateOnly JoinDate { get; set; }

        [@Column("role")]
        public string Role { get; set; } = string.Empty;

        [@Column("photo")]
        public string? Photo { get; set; }

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
