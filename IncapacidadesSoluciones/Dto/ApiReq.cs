using System.ComponentModel.DataAnnotations;

namespace IncapacidadesSoluciones.Dto
{
    public class ApiReq<T>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Error en la petición")]
        public T Data { get; set; }
    }
}
