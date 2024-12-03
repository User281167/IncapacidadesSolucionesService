using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserExists(string email, string cedula);
        Task<bool> UserExists(Guid id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetById(Guid id);
        Task<User> SignUp(string email, string password);
        Task<User> Update(User user);
        Task<User> UpdateByEmail(User user);
        Task<User> SignIn(string email, string password);
        Task<Collaborator> GetCollaboratorById(Guid id);
        Task<Collaborator> UpdateCollaborator(Collaborator collaborator);
        Task Delete(Guid id);
        Task<User> GetByEmailOrCedula(string email, string cedula);
        Task<List<User>> GetByNameOrCedula(string nit, string name, string lastName, string cedula);
        Task<List<UserInfoRes>> GetCollaboratorByNameOrCedula(string nit, string name, string lastName, string cedula);
        Task<List<User>> GetSpecialRoles(string nit);
        Task<List<Notification>> GetNotifications(Guid id);
    }
}
