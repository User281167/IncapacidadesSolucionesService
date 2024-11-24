using IncapacidadesSoluciones.Models;
using QueryOptions = Supabase.Postgrest.QueryOptions;

namespace IncapacidadesSoluciones.Repositories
{
    public class AccessCodeRepository : IAccessCodeRepository
    {
        private readonly Supabase.Client client;

        public AccessCodeRepository(Supabase.Client client)
        {
            this.client = client;
        }

        public async Task<AccessCode> GetLoginCodeById(Guid id)
        {
            var res = await client
                .From<AccessCode>()
                .Where(loginCode => loginCode.Id == id)
                .Single();

            return res;
        }

        public async Task<AccessCode> GetLoginCodeByCode(string code)
        {
            var res = await client
                .From<AccessCode>()
                .Where(loginCode => loginCode.Code == code)
                .Single();

            return res;
        }

        public async Task<AccessCode> Insert(AccessCode loginCode)
        {
            var res = await client
                .From<AccessCode>()
                .Insert(loginCode, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.First();
        }

        public async Task<bool> LoginCodeExists(string code)
        {
            var res = await GetLoginCodeByCode(code);
            return res != null;
        }

        public async Task<AccessCode> Update(AccessCode loginCode)
        {
            var res = await client
                .From<AccessCode>()
                .Where(item => item.Id == loginCode.Id)
                .Update(loginCode);

            return res.Models.First();
        }
    }
}
