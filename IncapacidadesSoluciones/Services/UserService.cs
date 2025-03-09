using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Utilities.Role;
using IncapacidadesSoluciones.Dto.auth;

namespace IncapacidadesSoluciones.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;
        private readonly IInabilityRepository inabilityRepository;

        public UserService(IUserRepository userRepository, IInabilityRepository inabilityRepository)
        {
            this.userRepository = userRepository;
            this.inabilityRepository = inabilityRepository;
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

        public async Task<ApiRes<String>> UpdatePhoto(Guid userId, IFormFile file)
        {
            try
            {
                string path = await userRepository.UpdatePhoto(userId, file);

                if (path == null)
                    return new ApiRes<String>() { Message = "Error al actualizar la foto." };

                return new ApiRes<String>() { Success = true, Data = path };

            }
            catch (Exception ex)
            {
                return new ApiRes<String>() { Message = "Error al actualizar la foto. Solo se aceptan archivos jpeg y png, máximo de 2MB." };
            }
        }

        public async Task<ApiRes<String>> GetPhotoUrl(Guid userId)
        {
            string path = await userRepository.GetPhotoUrl(userId);

            if (path == null)
                return new ApiRes<String>() { Message = "Error al obtener la foto." };

            return new ApiRes<String>() { Success = true, Data = path };
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

        public async Task<ApiRes<Collaborator>> GetCollaborator(Guid id)
        {
            Collaborator user = await userRepository.GetCollaboratorById(id);

            if (user == null)
                return new ApiRes<Collaborator>() { Message = "No se encuentra el usuario por el ID dado." };

            return new ApiRes<Collaborator>() { Success = true, Data = user };
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


        public async Task<ApiRes<User>> CreateRole(AuthRoleReq req)
        {
            USER_ROLE role = UserRoleFactory.GetRole(req.Role);

            if (role == USER_ROLE.NOT_FOUND)
                return new ApiRes<User> { Message = $"No se puede crear un usuario con credenciales de {req.Role}." };
            else if (role == USER_ROLE.LEADER)
                return new ApiRes<User> { Message = "No se puede crear un usuario con credenciales de LIDER." };
            else if (role == USER_ROLE.COLLABORATOR)
                return new ApiRes<User> { Message = "No se puede crear un usuario con credenciales de COLABORADOR." };

            // check leader and get company nit
            var leader = await userRepository.GetById(req.LeaderId);

            if (leader == null)
                return new ApiRes<User> { Message = "Compruebe que tengas las credenciales necesarias para asignar roles." };
            else if (await userRepository.UserExists(req.Email, req.Cedula))
                return new ApiRes<User> { Message = "Ya existe un usuario con ese correo o cédula registrado." };

            var user = await userRepository.SignUp(req.Email, req.Password);

            if (user == null)
                return new ApiRes<User> { Message = "Error al registrar el usuario" };

            user.Name = req.Name;
            user.LastName = req.LastName;
            user.Email = req.Email.ToLower();
            user.Cedula = req.Cedula;
            user.Phone = req.Phone;
            user.Role = UserRoleFactory.GetRoleName(role);
            user.CompanyNIT = leader.CompanyNIT;

            var res = await userRepository.UpdateByEmail(user);

            if (res == null)
                return new ApiRes<User> { Message = "Error al registrar y actualizar el usuario" };

            return new ApiRes<User>
            {
                Success = true,
                Message = "Usuario creado con éxito.",
                Data = res
            };
        }

        public async Task<ApiRes<User>> UpdateRole(AuthRoleReq req)
        {
            if (req.UserId == null)
                return new ApiRes<User> { Message = "El campo Id es obligatorio." };

            var role = UserRoleFactory.GetRole(req.Role);

            if (role == USER_ROLE.NOT_FOUND)
                return new ApiRes<User> { Message = $"No se puede crear un usuario con credenciales de {req.Role}." };
            else if (role == USER_ROLE.LEADER)
                return new ApiRes<User> { Message = "No se puede crear un usuario con credenciales de LIDER." };
            else if (role == USER_ROLE.COLLABORATOR)
                return new ApiRes<User> { Message = "No se puede crear un usuario con credenciales de COLABORADOR." };

            // check leader and get company nit
            var leader = await userRepository.GetById(req.LeaderId);

            if (leader == null)
                return new ApiRes<User>
                {
                    Message = "Compruebe que tengas las credenciales necesarias para asignar roles."
                };

            var user = await userRepository.GetById(req.UserId ?? Guid.Empty);
            var UserExists = await userRepository.GetByEmailOrCedula(req.Email, req.Cedula);

            if (user == null)
                return new ApiRes<User> { Message = "No se pudo encontrar el usuario." };
            else if (UserExists != null && UserExists.Id != user.Id)
                return new ApiRes<User>
                {
                    Message = "Ya existe un usuario con ese correo o cédula registrado."
                };

            user.Name = req.Name;
            user.LastName = req.LastName;
            user.Email = req.Email.ToLower();
            user.Cedula = req.Cedula;
            user.Phone = req.Phone;
            user.Role = UserRoleFactory.GetRoleName(role);
            user.CompanyNIT = leader.CompanyNIT;

            var res = await userRepository.Update(user);

            if (res == null)
                return new ApiRes<User> { Message = "Error al actualizar el usuario" };

            return new ApiRes<User>
            {
                Success = true,
                Message = "Usuario actualizado con éxito.",
                Data = res
            };
        }

        public async Task<ApiRes<bool>> DeleteRole(DeleteAuthReq req)
        {
            var leader = await userRepository.GetById(req.LeaderId);

            if (leader == null)
                return new ApiRes<bool> { Message = "No se pudo encontrar el líder." };

            var user = await userRepository.GetById(req.UserId);

            if (user == null)
                return new ApiRes<bool> { Message = "No se pudo encontrar el usuario." };
            else if (leader.Id == user.Id)
                return new ApiRes<bool> { Message = "No se puede eliminar el usuario que es el líder." };
            else if (leader.CompanyNIT != user.CompanyNIT)
                return new ApiRes<bool>
                {
                    Message = "No se puede eliminar el usuario que no pertenece a la empresa del líder."
                };

            await userRepository.Delete(user.Id);

            return new ApiRes<bool>
            {
                Success = true,
                Message = "Usuario eliminado con éxito.",
                Data = true
            };
        }

        public async Task<ApiRes<List<Notification>>> GetNotifications(Guid userId)
        {
            List<Notification> res = await userRepository.GetNotifications(userId);

            if (res == null)
                return new ApiRes<List<Notification>>() { Message = "Error al obtener los datos." };

            return new ApiRes<List<Notification>>() { Success = true, Data = res };
        }

        public async Task<ApiRes<Notification>> AddNotification(AddNotificationReq req)
        {
            User user = await userRepository.GetById(req.UserId);

            if (user == null)
                return new ApiRes<Notification>() { Message = "No se encuentra el usuario por el ID dado." };

            if (req.InabilityId != null)
            {
                Inability inability = await inabilityRepository.GetById(req.InabilityId.Value);

                if (inability == null)
                    return new ApiRes<Notification>() { Message = "No se encuentra la inactividad por el ID dado." };
                else if (req.UserId != inability.CollaboratorId)
                    return new ApiRes<Notification>() { Message = "No tienes permisos para realizar esta operación." };
            }

            Notification notification = new Notification()
            {
                Title = req.Title,
                Message = req.Message,
                UserId = req.UserId,
                InabilityId = req.InabilityId
            };

            Notification res = await userRepository.AddNotification(notification);

            if (res == null)
                return new ApiRes<Notification>() { Message = "Error al añadir la notificación." };

            return new ApiRes<Notification>() { Success = true, Data = res };
        }
    }
}
