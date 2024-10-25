namespace IncapacidadesSoluciones.Utilities.Company
{
    public class CompanySectorFactory
    {
        private static Dictionary<COMPANY_SECTOR, string> sectors = new Dictionary<COMPANY_SECTOR, string>()
        {
            { COMPANY_SECTOR.PRIMARY, "primario"},
            { COMPANY_SECTOR.SECONDARY_OR_INDUSTRIAL, "secundario o industrial" },
            { COMPANY_SECTOR.TERTIARY_OR_SERVICES, "terciario o servicios" }
        };

        public static string GetCompanySector(COMPANY_SECTOR companySector)
        {
            return sectors[companySector];
        }

        public static bool IsValid(string companySector)
        {
            return sectors.ContainsValue(companySector.ToLower());
        }
    }
}
