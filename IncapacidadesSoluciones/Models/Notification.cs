using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IncapacidadesSoluciones.Models
{
    [Table("notifications")]
    public class Notification : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("id_user")]
        public Guid UserId { get; set; }

        [Column("id_inability")]
        public Guid? InabilityId { get; set; } = default;

        [Column("title")]
        public string Title { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}