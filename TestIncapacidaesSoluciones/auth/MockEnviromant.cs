namespace TestIncapacidadesSoluciones.auth
{
    public interface IEnvironmentWrapper
    {
        string GetEnvironmentVariable(string key);
    }

    public class EnvironmentWrapper : IEnvironmentWrapper
    {
        public static string JWT_KEY_TEST = "afsdkjasjflxswafsdklk434orqiwup3457u-34oewir4irroqwiffv48mfs";

        public string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
