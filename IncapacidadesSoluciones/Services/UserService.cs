using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;

namespace IncapacidadesSoluciones.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> CreateUser(CreateUserReq userclient)
        {
            var newUser = User.FromDto(userclient);

            if (await userRepository.UserExists(newUser.Email, newUser.Cedula))
                return null;

            return newUser;
        }
    }
}
