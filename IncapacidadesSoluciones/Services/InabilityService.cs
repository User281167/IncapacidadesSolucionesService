using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Utilities;
using IncapacidadesSoluciones.Utilities.Role;

namespace IncapacidadesSoluciones.Services
{
    public class InabilityService
    {
        private readonly IInabilityRepository inabilityRepository;
        private readonly IUserRepository userRepository;

        public InabilityService(IInabilityRepository inabilityRepository, IUserRepository userRepository)
        {
            this.inabilityRepository = inabilityRepository;
            this.userRepository = userRepository;
        }

        public async Task<ApiRes<Inability>> AddInability(InabilityReq req)
        {
            if (req == null)
                return new ApiRes<Inability>() { Success = false, Message = "Información no válida." };
            else if (!await userRepository.UserExists(req.IdCollaborator))
                return new ApiRes<Inability>() { Success = false, Message = "No se encuentra el colaborador por el ID dado." };

            var inability = new Inability()
            {
                CollaboratorId = req.IdCollaborator,
                Description = req.Description,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                HealthEntity = req.HealthEntity,
                Type = req.Type,
            };

            var res = await inabilityRepository.Insert(inability);

            if (res == null)
                return new ApiRes<Inability>() { Success = false, Message = "Error al registrar la incapacidad." };

            return new ApiRes<Inability>()
            {
                Success = true,
                Data = res
            };
        }

        public async Task<ApiRes<List<Inability>>> GetAllFromUser(Guid id)
        {
            if (!await userRepository.UserExists(id))
                return new ApiRes<List<Inability>>() { Success = false, Message = "No se encuentra el colaborador por el ID dado." };

            var res = await inabilityRepository.GetUserInabilities(id);

            if (res == null)
                return new ApiRes<List<Inability>>() { Success = false, Message = "Error al obtener los datos." };

            return new ApiRes<List<Inability>>() { Success = true, Data = res };
        }

        public async Task<ApiRes<List<Inability>>> GetNoAccepted(Guid receptionistId)
        {
            var receptionist = await userRepository.GetById(receptionistId);

            if (receptionist == null)
                return new ApiRes<List<Inability>>() { Message = "No se encuentra el usuario por el ID dado." };

            var nit = receptionist.CompanyNIT;
            var res = await inabilityRepository.GetNoAccepted(nit);

            if (res == null)
                return new ApiRes<List<Inability>>() { Message = "Error al obtener los datos." };

            return new ApiRes<List<Inability>>() { Success = true, Data = res };
        }

        public async Task<ApiRes<Inability>> UpdateStateInability(Guid id, INABILITY_STATE state)
        {
            var inability = await inabilityRepository.GetById(id);

            if (inability == null)
                return new ApiRes<Inability>() { Message = "No se encuentra la incapacidad por el ID dado." };
            else if (inability.State == InabilityStateFactory.GetState(state))
                return new ApiRes<Inability>()
                {
                    Success = true,
                    Message = "La incapacidad ya ha sido actualizada.",
                    Data = inability
                };

            Collaborator collaborator = await userRepository.GetCollaboratorById(inability.CollaboratorId);

            if (collaborator == null)
                return new ApiRes<Inability>() { Message = "No se encuentra el usuario de la incapacidad." };
            else if (collaborator.IsReplacing && state == INABILITY_STATE.ACCEPTED)
                return new ApiRes<Inability>() { Message = "El usuario a incapacitar está reemplazando otra." };

            // update collaborator
            bool isAccepted = state == INABILITY_STATE.ACCEPTED;

            // update collaborator and replacement
            if (isAccepted != collaborator.Inability)
            {
                collaborator.Inability = isAccepted;

                if (state == INABILITY_STATE.TERMINATED && collaborator.HasReplacement)
                {
                    // update user and replacement
                    var replacement = await userRepository.GetCollaboratorById(inability.ReplacementId ?? Guid.Empty);

                    if (replacement == null)
                        return new ApiRes<Inability>() { Message = "Error el usuario de reemplazo no se encuentra." };

                    replacement.IsReplacing = false;

                    var updateReplacement = await userRepository.UpdateCollaborator(replacement);

                    if (updateReplacement == null)
                        return new ApiRes<Inability>() { Message = "Error al actualizar el usuario de reemplazo." };

                    collaborator.HasReplacement = false;
                }

                var updateCollaborator = await userRepository.UpdateCollaborator(collaborator);

                if (updateCollaborator == null)
                    return new ApiRes<Inability>() { Message = "Error al actualizar el usuario de la incapacidad." };
            }

            // update incapacity
            inability.State = InabilityStateFactory.GetState(state);
            var updateInability = await inabilityRepository.Update(inability);

            if (updateInability == null)
                return new ApiRes<Inability>() { Message = "Error al actualizar la incapacidad." };

            return new ApiRes<Inability>() { Success = true, Data = updateInability };
        }

        public async Task<ApiRes<Inability>> UpdateAdvice(Guid id, bool isAdvice)
        {
            var inability = await inabilityRepository.GetById(id);

            if (inability == null)
                return new ApiRes<Inability>() { Message = "No se encuentra la incapacidad por el ID dado." };
            else if (inability.State != InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED))
                return new ApiRes<Inability>()
                {
                    Message = "La incapacidad no ha sido aceptada.",
                    Data = inability
                };
            else if (isAdvice == inability.Advise)
            {
                return new ApiRes<Inability>()
                {
                    Success = true,
                    Message = "La incapacidad ya ha sido actualizada.",
                    Data = inability
                };
            }

            // update incapacity
            inability.Advise = isAdvice;
            var updateInability = await inabilityRepository.Update(inability);

            if (updateInability == null)
                return new ApiRes<Inability>() { Message = "Error al actualizar la incapacidad." };

            return new ApiRes<Inability>() { Success = true, Data = updateInability };
        }

        public async Task<ApiRes<Inability>> PaymentInability(InabilityPaymentReq req)
        {
            var leader = await userRepository.GetById(req.LeaderId);

            if (leader == null)
                return new ApiRes<Inability>() { Message = "No tienes permisos para realizar esta operación." };

            var inability = await inabilityRepository.GetById(req.Id);

            if (inability == null)
                return new ApiRes<Inability>() { Message = "No se encuentra la incapacidad por el ID dado." };

            var user = await userRepository.GetById(inability.CollaboratorId);

            if (user == null)
                return new ApiRes<Inability>() { Message = "No se encuentra el usuario por el ID dado." };
            else if (leader.CompanyNIT != user.CompanyNIT)
                return new ApiRes<Inability>() { Message = "No tienes permisos para realizar esta operación." };
            else if (inability.State != InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED))
                return new ApiRes<Inability>()
                {
                    Message = "La incapacidad no ha sido aceptada.",
                    Data = inability
                };

            // update
            inability.CompanyPayment = req.CompanyPayment;
            inability.HealthEntityPayment = req.HealthEntityPayment;
            inability.Pay = req.CompanyPayment + req.HealthEntityPayment;
            var updateInability = await inabilityRepository.Update(inability);

            if (updateInability == null)
                return new ApiRes<Inability>() { Message = "Error al actualizar la incapacidad." };

            return new ApiRes<Inability>() { Success = true, Data = updateInability };
        }

        public async Task<ApiRes<Inability>> AddReplacement(InabilityReplacementReq req)
        {
            var leader = await userRepository.GetById(req.LeaderId);

            if (leader == null)
                return new ApiRes<Inability>() { Message = "No tienes permisos para realizar esta operación." };

            var inability = await inabilityRepository.GetById(req.InabilityId);

            if (inability == null)
                return new ApiRes<Inability>() { Message = "No se encuentra la incapacidad por el ID dado." };
            else if (inability.State != InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED))
                return new ApiRes<Inability>()
                {
                    Message = "La incapacidad no ha sido aceptada.",
                    Data = inability
                };

            var user = await userRepository.GetById(inability.CollaboratorId);

            if (user == null)
                return new ApiRes<Inability>() { Message = "No se encuentra el usuario por el ID dado." };
            else if (user.CompanyNIT != leader.CompanyNIT)
                return new ApiRes<Inability>() { Message = "No tienes permisos para realizar esta operación." };

            // update collaborator and replacement
            var collaborator = await userRepository.GetCollaboratorById(req.ReplacementId);
            var collaboratorInability = await userRepository.GetCollaboratorById(user.Id);

            if (collaborator == null || collaboratorInability == null)
                return new ApiRes<Inability>() { Message = "No se encuentra el reemplazo por el ID dado." };
            else if (user.Id == collaborator.Id)
                return new ApiRes<Inability>() { Message = "El usuario de reemplazo es el mismo que el usuario de la incapacidad." };
            else if (collaborator.Inability)
                return new ApiRes<Inability>() { Message = "El usuario de reemplazo tiene una incapacidad aceptada." };
            else if (collaborator.IsReplacing)
                return new ApiRes<Inability>() { Message = "El usuario de reemplazo ya está reemplazando otra." };
            else if (collaboratorInability.IsReplacing)
                return new ApiRes<Inability>() { Message = "El usuario de la incapacidad está reemplazando otra." };

            // update incapacity
            inability.ReplacementId = req.ReplacementId;
            collaboratorInability.Inability = true;
            collaboratorInability.HasReplacement = true;
            collaborator.IsReplacing = true;

            var updateInability = await inabilityRepository.Update(inability);
            var updateCollaborator = await userRepository.UpdateCollaborator(collaborator);
            var updateUser = await userRepository.UpdateCollaborator(collaboratorInability);

            if (updateInability == null)
                return new ApiRes<Inability>() { Message = "Error al actualizar la incapacidad." };
            else if (updateCollaborator == null)
                return new ApiRes<Inability>() { Message = "Error al actualizar el usuario del reemplazo." };
            else if (updateUser == null)
                return new ApiRes<Inability>() { Message = "Error al actualizar el usuario de la incapacidad." };

            return new ApiRes<Inability>() { Success = true, Data = updateInability };
        }

        public async Task<ApiRes<List<InabilityPaymentRes>>> GetPaymentReport(Guid id)
        {
            User special = await userRepository.GetById(id);

            if (special == null)
                return new ApiRes<List<InabilityPaymentRes>>()
                {
                    Message = "No tienes permisos para realizar esta operación."
                };


            List<InabilityPaymentRes> res = await inabilityRepository.GetPaymentReport(special.CompanyNIT);

            if (res == null)
                return new ApiRes<List<InabilityPaymentRes>>() { Message = "Error al obtener los datos." };

            return new ApiRes<List<InabilityPaymentRes>>() { Success = true, Data = res };
        }

        public async Task<ApiRes<List<Notification>>> GetNotifications(Guid id)
        {
            var res = await inabilityRepository.GetNotifications(id);

            if (res == null)
                return new ApiRes<List<Notification>>() { Message = "Error al obtener los datos." };

            return new ApiRes<List<Notification>>() { Success = true, Data = res };
        }

        public async Task<ApiRes<InabilityFile>> AddFile(AddFileReq req)
        {
            if (req.InabilityId != null)
            {
                var inability = await inabilityRepository.GetById(req.InabilityId.Value);

                if (inability == null)
                    return new ApiRes<InabilityFile>() { Message = "No se encuentra la incapacidad por el ID dado." };
                else if (req.UserId != inability.CollaboratorId)
                    return new ApiRes<InabilityFile>() { Message = "El usuario no es el responsable de la incapacidad." };
            }

            var res = await inabilityRepository.AddFile(req);

            if (res == null)
                return new ApiRes<InabilityFile>() { Message = "Error al añadir el archivo." };

            return new ApiRes<InabilityFile>() { Success = true, Data = res };
        }

        public async Task<ApiRes<List<InabilityFile>>> GetFiles(Guid id)
        {
            // return url
            var res = await inabilityRepository.GetFiles(id);
            return res == null ?
            new ApiRes<List<InabilityFile>>() { Message = "No se encontraron archivos." } :
            new ApiRes<List<InabilityFile>>() { Success = true, Data = res };
        }
    }
}