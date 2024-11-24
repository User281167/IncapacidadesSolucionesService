using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface IAccessCodeRepository
    {
        public Task<AccessCode> GetLoginCodeById(Guid id);
        public Task<AccessCode> GetLoginCodeByCode(string code);
        public Task<bool> LoginCodeExists(string code);
        public Task<AccessCode> Insert(AccessCode loginCode);
        public Task<AccessCode> Update(AccessCode loginCode);
    }
}
