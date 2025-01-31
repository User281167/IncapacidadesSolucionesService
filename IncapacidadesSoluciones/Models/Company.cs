using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IncapacidadesSoluciones.Models
{
    [@Table("companies")]
    public class Company : BaseModel
    {
        [@PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("join_date", ignoreOnInsert: true, ignoreOnUpdate: true)]
        public DateOnly JoinDate { get; set; }

        [@Column("nit")]
        public string Nit { get; set; }

        [@Column("name")]
        public string Name { get; set; }

        [@Column("description")]
        public string? Description { get; set; }

        [@Column("email")]
        public string Email { get; set; }

        [@Column("created_at")]
        public DateOnly? Founded { get; set; }

        [@Column("address")]
        public string? Address { get; set; }

        [@Column("type")]
        public string Type { get; set; }

        [@Column("sector")]
        public string Sector { get; set; }

        [Column("leader_id")]
        public Guid LeaderId { get; set; }
    }
}
