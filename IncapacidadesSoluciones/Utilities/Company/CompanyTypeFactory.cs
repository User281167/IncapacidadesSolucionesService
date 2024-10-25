namespace IncapacidadesSoluciones.Utilities.Company
{
    public class CompanyTypeFactory
    {
        private static Dictionary<COMPANY_TYPE, string> types = new Dictionary<COMPANY_TYPE, string>()
        {
            { COMPANY_TYPE.SMALL, "pequeña" },
            { COMPANY_TYPE.MEDIUM, "mediana" },
            { COMPANY_TYPE.BIG, "grande" },
            { COMPANY_TYPE.NON_PROFIT_ORGANIZATION, "organización sin fines de lucro" }
        };

        public static string GetCompanyType(COMPANY_TYPE companyType)
        {
            return types[companyType];
        }

        public static bool IsValid(string companyType)
        {
            return types.Any(x => x.Value == companyType.ToLower());
        }
    }
}
