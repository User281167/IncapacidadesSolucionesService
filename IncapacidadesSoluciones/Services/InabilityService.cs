using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;

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
    }
}
