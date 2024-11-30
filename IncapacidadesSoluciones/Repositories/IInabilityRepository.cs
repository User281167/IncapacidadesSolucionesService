using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface IInabilityRepository
    {
        Task<Inability> Insert(Inability inability);
        Task<List<Inability>> GetUserInabilities(Guid id);
        Task<List<Inability>> GetNotAccepted(string nit);
    }
}
