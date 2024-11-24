using IncapacidadesSoluciones.Models;
using QueryOptions = Supabase.Postgrest.QueryOptions;

namespace IncapacidadesSoluciones.Repositories
{
    public class LoginCodeRepository : ILoginCodeRepository
    {
        private readonly Supabase.Client client;

        public LoginCodeRepository(Supabase.Client client)
        {
            this.client = client;
        }

        public async Task<LoginCode> GetLoginCodeById(Guid id)
        {
            var res = await client
                .From<LoginCode>()
                .Where(loginCode => loginCode.Id == id)
                .Single();

            return res;
        }

        public async Task<LoginCode> GetLoginCodeByCode(string code)
        {
            var res = await client
                .From<LoginCode>()
                .Where(loginCode => loginCode.Code == code)
                .Single();

            return res;
        }

        public async Task<LoginCode> Insert(LoginCode loginCode)
        {
            var res = await client
                .From<LoginCode>()
                .Insert(loginCode, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.First();
        }

        public async Task<bool> LoginCodeExists(string code)
        {
            var res = await GetLoginCodeByCode(code);
            return res != null;
        }

        public async Task<LoginCode> Update(LoginCode loginCode)
        {
            var res = await client
                .From<LoginCode>()
                .Where(item => item.Id == loginCode.Id)
                .Update(loginCode);

            return res.Models.First();
        }
    }
}
