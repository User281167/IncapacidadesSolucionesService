using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Services
{
    public class UserService
    {
        public async Task<User> CreateUser(CreateUserReq user, Supabase.Client client)
        {
            var newUser = User.FromDto(user);
            var response = await client.From<User>().Insert(newUser);
            return response.Models.First();
        }
    }
}
