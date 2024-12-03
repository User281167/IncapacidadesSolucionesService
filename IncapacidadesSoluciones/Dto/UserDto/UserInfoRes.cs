using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Dto.UserDto
{
    public class UserInfoRes
    {
        public User User { get; set; }
        public Collaborator Collaborator { get; set; }
    }
}