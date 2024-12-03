using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IncapacidadesSoluciones.Models
{
    [Table("files")]
    public class InabilityFile : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("id_user")]
        public Guid UserId { get; set; }

        [Column("id_inability")]
        public Guid? InabilityId { get; set; } = default;

        [Column("title")]
        public string Title { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }
    }
}