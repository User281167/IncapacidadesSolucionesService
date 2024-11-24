using Supabase.Postgrest.Models;

using Table = Supabase.Postgrest.Attributes.TableAttribute;
using Column = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IncapacidadesSoluciones.Models
{
    [Table("access_codes")]
    public class AccessCode : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("nit")]
        public string NIT { get; set; }

        [Column("fecha_expiracion")]
        public DateOnly ?ExpirationDate { get; set; }

        public static string GenerateCode(string companyName, int length = 6)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            
            return companyName + "-" + code;
        }
    }
}
