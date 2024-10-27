using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IncapacidadesSoluciones.Models
{
    [@Table("empresas")]
    public class Company : BaseModel
    {
        [@PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("fecha_union", ignoreOnInsert: true, ignoreOnUpdate: true)]
        public DateOnly JoinDate { get; set; }

        [@Column("nit")]
        public string Nit { get; set; }
        
        [@Column("nombre")]
        public string Name { get; set; }

        [@Column("descripcion")]
        public string Description { get; set; }

        [@Column("email")]
        public string Email { get; set; }

        [@Column("creacion")]
        public DateOnly Founded { get; set; }

        [@Column("direccion")]
        public string Address { get; set; }

        [@Column("tipo")]
        public string Type { get; set; }

        [@Column("sector")]
        public string Sector { get; set; }

        [Column("lider_id")]
        public Guid LeaderId { get; set; }
    }
}
