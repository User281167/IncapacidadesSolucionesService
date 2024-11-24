using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface ICompanyRepository
    {
        Task<Boolean> CompanyExists(string nit);
        Task<Company> Insert(Company company);
        Task<Company> GetCompanyByNit(string nit);
        Task<Company> GetCompany(Guid id);
    }
}
