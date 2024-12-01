using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Utilities;

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
                IdCollaborator = req.IdCollaborator,
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

            Collaborator collaborator = await userRepository.GetCollaboratorById(inability.IdCollaborator);

            if (collaborator == null)
                return new ApiRes<Inability>() { Message = "No se encuentra el usuario de la incapacidad." };

            // update collaborator
            bool isAccepted = state == INABILITY_STATE.ACCEPTED;

            if (isAccepted != collaborator.Inability)
            {
                collaborator.Inability = state == INABILITY_STATE.ACCEPTED;
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
    }
}