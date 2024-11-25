using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IncapacidadesSoluciones.Models
{
    [Table("inabilities")]
    public class Inability: BaseModel
    {
        [PrimaryKey("Id", false)]
        public Guid Id { get; set; }

        [Column("id_collaborator")]
        public Guid IdCollaborator { get; set; }

        [Column("id_replacement")]
        public Guid ?IdReplacement { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("start_date")]
        public DateOnly StartDate { get; set; }

        [Column("end_date")]
        public DateOnly EndDate { get; set; }

        [Column("health_entity")]
        public string HealthEntity { get; set; }

        [Column("pay")]
        public ulong Pay { get; set; }

        [Column("company_payment")]
        public ulong CompanyPayment { get; set; }

        [Column("health_entity_payment")]
        public ulong HealthEntityPayment { get; set; }

        [Column("accepted")]
        public bool Accepted { get; set; }

        [Column("finished")]
        public bool Finished {  get; set; }

        [Column("discharged")]
        public bool Discharged { get; set; }

        [Column("advise")]
        public bool Advise { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
