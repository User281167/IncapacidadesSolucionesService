using IncapacidadesSoluciones.Models;

using QueryOptions = Supabase.Postgrest.QueryOptions;

namespace IncapacidadesSoluciones.Repositories
{
    public class InabilityRepository: IInabilityRepository
    {
        private readonly Supabase.Client client;

        public InabilityRepository(Supabase.Client client)
        {
            this.client = client;
        }

        public async Task<Inability> Insert(Inability inability)
        {
            var res = await client
                .From<Inability>()
                .Insert(inability, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.First();
        }
    }
}
