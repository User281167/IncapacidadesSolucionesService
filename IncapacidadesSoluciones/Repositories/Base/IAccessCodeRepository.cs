using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface IAccessCodeRepository
    {
        public Task<AccessCode> GetById(Guid id);
        public Task<AccessCode> GetByCode(string code);
        public Task<bool> Exists(string code);
        public Task<AccessCode> Insert(AccessCode loginCode);
        public Task<AccessCode> Update(AccessCode loginCode);
    }
}
