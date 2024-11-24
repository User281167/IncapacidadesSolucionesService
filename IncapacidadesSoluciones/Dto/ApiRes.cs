namespace IncapacidadesSoluciones.Dto
{
    public class ApiRes <T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
