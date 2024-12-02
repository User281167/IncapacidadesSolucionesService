using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Dto.UserDto;

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
    }
}
