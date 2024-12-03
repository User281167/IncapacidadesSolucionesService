using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface IInabilityRepository
    {
        Task<Inability> Insert(Inability inability);
        Task<List<Inability>> GetUserInabilities(Guid id);
        Task<List<Inability>> GetNoAccepted(string nit);
        Task<Inability> GetById(Guid id);
        Task<Inability> Update(Inability inability);
        Task<List<InabilityPaymentRes>> GetPaymentReport(string nit);
    }
}
