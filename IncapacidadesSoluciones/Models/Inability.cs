﻿using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IncapacidadesSoluciones.Models
{
    [Table("inabilities")]
    public class Inability : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("id_collaborator")]
        public Guid CollaboratorId { get; set; }

        [Column("id_replacement")]
        public Guid? ReplacementId { get; set; }

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

        [Column("state")]
        public string State { get; set; }

        [Column("advise")]
        public bool Advise { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
