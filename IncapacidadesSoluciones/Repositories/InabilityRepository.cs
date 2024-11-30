using IncapacidadesSoluciones.Models;
using QueryOptions = Supabase.Postgrest.QueryOptions;
using Operator = Supabase.Postgrest.Constants.Operator;

namespace IncapacidadesSoluciones.Repositories
{
    public class InabilityRepository : IInabilityRepository
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

        public async Task<List<Inability>> GetUserInabilities(Guid id)
        {
            var res = await client
                .From<Inability>()
                .Where(i => i.IdCollaborator == id)
                .Get();

            return res.Models;
        }

        public async Task<List<Inability>> GetNotAccepted(string nit)
        {
            var users = await client
                .From<User>()
                .Where(item => item.CompanyNIT == nit)
                .Get();

            var ids = users.Models.Select(u => u.Id).ToList();

            var res = await client
                .From<Inability>()
                .Filter("id_collaborator", Operator.In, ids)
                .Where(item => item.Accepted == false && item.Discharged == false)
                .Get();

            return res.Models;
        }
    }
}
