using IncapacidadesSoluciones.Models;

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
            if (session.User == null)
                return null;

            var user = await GetUserByEmail(session.User.Email);
            return user;
        }

        public async Task<User> Update(User user)
        {
            var res = await client
                    .From<User>()
                    .Where(u => u.Email == user.Email)
                    .Update(user);

            return res.Models.First();
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

            return res.Models.First();
        }

        public async Task<Collaborator> UpdateCollaborator(Collaborator collaborator)
        {
            var res = await client
                .From<Collaborator>()
                .Where(c => c.Id == collaborator.Id)
                .Update(collaborator);

            return res.Models.First();
        }
    }
}
