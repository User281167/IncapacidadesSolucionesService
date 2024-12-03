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

        public async Task<ApiRes<List<User>>> SearchUser(Guid searchBy, string name, string lastName, string cedula)
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

        public async Task<ApiRes<List<UserInfoRes>>> SearchCollaborator(Guid searchBy, string name, string lastName, string cedula)
        {
            User userSpecial = await userRepository.GetById(searchBy);

            if (userSpecial == null)
                return new ApiRes<List<UserInfoRes>>()
                {
                    Message = "No tienes permisos para realizar esta operación."
                };

            USER_ROLE role = UserRoleFactory.GetRole(userSpecial.Role);

            if (role == USER_ROLE.NOT_FOUND || role == USER_ROLE.COLLABORATOR)
                return new ApiRes<List<UserInfoRes>>()
                {
                    Message = "No tienes permisos para realizar esta operación."
                };

            var collaborators = await userRepository.GetCollaboratorByNameOrCedula(userSpecial.CompanyNIT, name, lastName, cedula);

            if (collaborators == null)
                return new ApiRes<List<UserInfoRes>>() { Message = "Error al obtener los datos." };

            var res = await userRepository.GetCollaboratorByNameOrCedula(userSpecial.CompanyNIT, name, lastName, cedula);

            if (res == null)
                return new ApiRes<List<UserInfoRes>>() { Message = "Error al obtener los datos." };

            return new ApiRes<List<UserInfoRes>>() { Success = true, Data = res };
        }

        public async Task<ApiRes<List<User>>> GetSpecialRoles(Guid leaderId)
        {
            User leader = await userRepository.GetById(leaderId);

            if (leader == null)
                return new ApiRes<List<User>>() { Message = "No tienes permisos para realizar esta operación." };
            else if (UserRoleFactory.GetRole(leader.Role) != USER_ROLE.LEADER)
                return new ApiRes<List<User>>() { Message = "No tienes permisos para realizar esta operación." };


            var res = await userRepository.GetSpecialRoles(leader.CompanyNIT);

            if (res == null)
                return new ApiRes<List<User>>() { Message = "Error al obtener los datos." };

            return new ApiRes<List<User>>() { Success = true, Data = res };
        }

        public async Task<ApiRes<List<Notification>>> GetNotifications(Guid userId)
        {
            List<Notification> res = await userRepository.GetNotifications(userId);

            if (res == null)
                return new ApiRes<List<Notification>>() { Message = "Error al obtener los datos." };

            return new ApiRes<List<Notification>>() { Success = true, Data = res };
        }
    }
}
