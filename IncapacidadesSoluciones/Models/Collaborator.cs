using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IncapacidadesSoluciones.Models
{
    [@Table("collaborators")]
    public class Collaborator : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [@Column("position")]
        public string Position { get; set; }

        [@Column("health_entity")]
        public string HealthEntity { get; set; }

        [@Column("inability")]
        public bool Inability { get; set; }

        [@Column("has_replacement")]
        public bool HasReplacement { get; set; }

        [@Column("is_replacing")]
        public bool IsReplacing { get; set; }

        [@Column("banking_entity")]
        public string BankingEntity { get; set; }

        [@Column("bank_account")]
        public string BankAccount { get; set; }
    }
}