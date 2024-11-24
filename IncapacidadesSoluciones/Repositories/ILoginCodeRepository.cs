using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface ILoginCodeRepository
    {
        public Task<LoginCode> GetLoginCodeById(Guid id);
        public Task<LoginCode> GetLoginCodeByCode(string code);
        public Task<bool> LoginCodeExists(string code);
        public Task<LoginCode> Insert(LoginCode loginCode);
        public Task<LoginCode> Update(LoginCode loginCode);
    }
}
