using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Utilities.Role;

namespace IncapacidadesSoluciones.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<ApiRes<User>> UpdateUser(UserReq user)
        {
            if (user == null)
                return new ApiRes<User>() { Success = false, Message = "Información no válida." };

            var newUser = await userRepository.GetById(user.Id);

            if (newUser == null)
                return new ApiRes<User>() { Success = false, Message = "No se encuentra el usuario por el ID dado." };
            else if (newUser.Cedula != user.Cedula && await userRepository.UserExists("", user.Cedula))
                return new ApiRes<User>() { Success = false, Message = "Credenciales incorrectas, verifique que la cédula sea unica." };

            newUser.Name = user.Name;
            newUser.LastName = user.LastName;
            newUser.Cedula = user.Cedula;
            newUser.Phone = user.Phone;

            var res = await userRepository.UpdateByEmail(newUser);

            if (res == null)
                return new ApiRes<User>() { Success = false, Message = "Error al actualizar el usuario." };

            return new ApiRes<User>()
            {
                Success = true,
                Data = res
            };
        }

        public async Task<ApiRes<User>> GetUserInfo(Guid userId, Guid searchBy)
        {
            User userSpecial = await userRepository.GetById(searchBy);

            if (userSpecial == null)
                return new ApiRes<User>() { Message = "No tienes permisos para realizar esta operación." };

            USER_ROLE role = UserRoleFactory.GetRole(userSpecial.Role);

            if (role == USER_ROLE.NOT_FOUND || role == USER_ROLE.COLLABORATOR)
                return new ApiRes<User>() { Message = "No tienes permisos para realizar esta operación." };

            User user = await userRepository.GetById(userId);

            if (user == null)
                return new ApiRes<User>() { Message = "No se encuentra el usuario por el ID dado." };
            else if (user.CompanyNIT != userSpecial.CompanyNIT)
                return new ApiRes<User>() { Message = "No tienes permisos para realizar esta operación." };

            return new ApiRes<User>() { Success = true, Data = user };
        }

        public async Task<ApiRes<List<User>>> SearchUserByNameOrCedula(Guid searchBy, string name, string lastName, string cedula)
        {
            User userSpecial = await userRepository.GetById(searchBy);

            if (userSpecial == null)
                return new ApiRes<List<User>>() { Message = "No tienes permisos para realizar esta operación." };

            USER_ROLE role = UserRoleFactory.GetRole(userSpecial.Role);

            if (role == USER_ROLE.NOT_FOUND || role == USER_ROLE.COLLABORATOR)
                return new ApiRes<List<User>>() { Message = "No tienes permisos para realizar esta operación." };

            var users = await userRepository.GetByNameOrCedula(userSpecial.CompanyNIT, name, lastName, cedula);

            if (users == null)
                return new ApiRes<List<User>>() { Message = "Error al obtener los datos." };

            return new ApiRes<List<User>>() { Success = true, Data = users };
        }
    }
}
