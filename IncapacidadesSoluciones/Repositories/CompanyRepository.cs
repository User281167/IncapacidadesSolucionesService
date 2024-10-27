using IncapacidadesSoluciones.Models;

using QueryOptions = Supabase.Postgrest.QueryOptions;

namespace IncapacidadesSoluciones.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly Supabase.Client client;

        public CompanyRepository(Supabase.Client client)
        {
            this.client = client;
        }

        public async Task<Boolean> CompanyExists(string nit)
        {
            var res = await client
                .From<Company>()
                .Where(company => company.Nit == nit)
                .Single();

            return res != null;
        }

        public async Task<Company> Insert(Company company)
        {
            var res = await client
                .From<Company>()
                .Insert(company, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.First();
        }
    }
}
