namespace IncapacidadesSoluciones.Dto
{
    public class ApiRes <T>
    {
        public T? Data { get; set; } = default;
        public bool Success { get; set; } = false;
        public string Message { get; set; }
    }
}
