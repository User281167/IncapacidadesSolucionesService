using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface IUserRepository
    {
        Task<Boolean> UserExists(string email, string cedula);
        Task<Boolean> UserExists(Guid id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetById(Guid id);
        Task<User> SignUp(string email, string password);
        Task<User> Update(User user);
        Task<User> UpdateByEmail(User user);
        Task<User> SignIn(string email, string password);
        Task<Collaborator> GetCollaboratorById(Guid id);
        Task<Collaborator> UpdateCollaborator(Collaborator collaborator);
        Task Delete(Guid id);
    }
}
