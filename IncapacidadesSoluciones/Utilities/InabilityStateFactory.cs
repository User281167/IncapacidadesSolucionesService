namespace IncapacidadesSoluciones.Utilities
{
    public class InabilityStateFactory
    {
        private static Dictionary<INABILITY_STATE, string> types = new Dictionary<INABILITY_STATE, string>()
        {
            { INABILITY_STATE.PENDING, "pendiente" },
            { INABILITY_STATE.ACCEPTED, "aceptada" },
            { INABILITY_STATE.DISCHARGED, "rechazada" },
            { INABILITY_STATE.FINISHED, "terminada" },
            { INABILITY_STATE.TERMINATED, "dada de baja" },
        };

        public static string GetState(INABILITY_STATE companyType)
        {
            return types[companyType];
        }

        public static INABILITY_STATE GetState(string companyType)
        {
            var item = types.SingleOrDefault(
                r => r.Value == companyType.ToLower(),
                new KeyValuePair<INABILITY_STATE, string>(INABILITY_STATE.PENDING, "")
                );

            return item.Key;
        }
    }
}
