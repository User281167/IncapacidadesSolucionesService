using IncapacidadesSoluciones.Models;
using Operator = Supabase.Postgrest.Constants.Operator;

namespace IncapacidadesSoluciones.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Supabase.Client client;

        public UserRepository(Supabase.Client client)
        {
            this.client = client;
        }

        public async Task<Boolean> UserExists(string email, string cedula)
        {
            var res = await client
                .From<User>()
                .Where(user => user.Email == email || user.Cedula == cedula)
                .Single();

            return res != null;
        }

        public async Task<Boolean> UserExists(Guid id)
        {
            return await GetById(id) != null;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await client
                    .From<User>()
                    .Where(u => u.Email == email)
                    .Single();

            return user;
        }

        public async Task<User> GetById(Guid id)
        {
            var user = await client
                    .From<User>()
                    .Where(u => u.Id == id)
                    .Single();

            return user;
        }

        public async Task<User> SignUp(string email, string password)
        {
            var session = await client.Auth.SignUp(email, password);

            if (session == null)
                return null;
            else if (session.User == null)
                return null;

            return await GetUserByEmail(session.User.Email);
        }

        public async Task<User> Update(User user)
        {
            var res = await client
                    .From<User>()
                    .Where(u => u.Id == user.Id)
                    .Update(user);

            return res.Models.FirstOrDefault();
        }

        public async Task<User> UpdateByEmail(User user)
        {
            var res = await client
                    .From<User>()
                    .Where(u => u.Email == user.Email)
                    .Update(user);

            return res.Models.FirstOrDefault();
        }

        public async Task<User> SignIn(string email, string password)
        {
            var session = await client.Auth.SignIn(email, password);

            if (session == null || session.User == null)
                return null;

            return await GetUserByEmail(session.User.Email);
        }

        public async Task<Collaborator> GetCollaboratorById(Guid id)
        {
            var res = await client
                .From<Collaborator>()
                .Where(c => c.Id == id)
                .Get();

            return res.Models.FirstOrDefault();
        }

        public async Task<Collaborator> UpdateCollaborator(Collaborator collaborator)
        {
            var res = await client
                .From<Collaborator>()
                .Where(c => c.Id == collaborator.Id)
                .Update(collaborator);

            return res.Models.FirstOrDefault();
        }

        public async Task Delete(Guid id)
        {
            await client
                .From<User>()
                .Where(c => c.Id == id)
                .Delete();
        }

        public async Task<User> GetByEmailOrCedula(string email, string cedula)
        {
            var user = await client
                .From<User>()
                .Where(u => u.Email == email || u.Cedula == cedula)
                .Single();

            return user;
        }
    }
}
