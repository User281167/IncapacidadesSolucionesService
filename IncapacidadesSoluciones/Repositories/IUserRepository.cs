using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface IUserRepository
    {
        Task<Boolean> UserExists(string email, string cedula);
        Task<Boolean> UserExists(Guid id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetById(Guid id);
        Task<User> SignUp(string email, string pasword);
        Task<User> Update(User user);
        Task<User> SignIn(string email, string password);
    }
}
