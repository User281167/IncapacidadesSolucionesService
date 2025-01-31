using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Utilities.Role;
using Operator = Supabase.Postgrest.Constants.Operator;
using QueryOptions = Supabase.Postgrest.QueryOptions;
using FileOptions = Supabase.Storage.FileOptions;
using Supabase.Storage;

namespace IncapacidadesSoluciones.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string USER_IMAGES_BUCKET = "user_images";

        private readonly Supabase.Client client;

        public UserRepository(Supabase.Client client)
        {
            this.client = client;
        }

        public async Task<bool> UserExists(string email, string cedula)
        {
            var res = await client
                .From<User>()
                .Where(user => user.Email == email || user.Cedula == cedula)
                .Single();

            return res != null;
        }

        public async Task<bool> UserExists(Guid id)
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

        public async Task<Collaborator> CreateCollaborator(Guid userId)
        {
            if (userId == Guid.Empty)
                return null;

            Collaborator collaborator = new() { Id = userId };

            var res = await client
                .From<Collaborator>()
                .Insert(collaborator, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.First();
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

        public async Task<List<User>> GetByNameOrCedula(string nit, string name, string lastName, string cedula)
        {
            var user = await client
                .From<User>()
                .Filter(u => u.CompanyNIT, Operator.Equals, nit)
                .Filter(u => u.Role, Operator.NotEqual, UserRoleFactory.GetRoleName(USER_ROLE.COLLABORATOR))
                .Filter(u => u.Name, Operator.ILike, $"%{name}%")
                .Filter(u => u.LastName, Operator.ILike, $"%{lastName}%")
                .Filter(u => u.Cedula, Operator.ILike, $"%{cedula}%")
                .Get();

            return user.Models;
        }

        public async Task<List<UserInfoRes>> GetCollaboratorByNameOrCedula(string nit, string name, string lastName, string cedula)
        {
            var users = await client
                .From<User>()
                .Filter(u => u.CompanyNIT, Operator.Equals, nit)
                .Filter(u => u.Role, Operator.Equals, UserRoleFactory.GetRoleName(USER_ROLE.COLLABORATOR))
                .Filter(u => u.Name, Operator.ILike, $"%{name}%")
                .Filter(u => u.LastName, Operator.ILike, $"%{lastName}%")
                .Filter(u => u.Cedula, Operator.ILike, $"%{cedula}%")
                .Get();

            if (users == null)
                return null;

            var ids = users.Models.Select(u => u.Id).ToList();

            var collaborator = await client
                .From<Collaborator>()
                .Filter("id", Operator.In, ids)
                .Get();

            if (collaborator == null)
                return null;

            List<UserInfoRes> res = new List<UserInfoRes>();

            foreach (var user in users.Models)
            {
                var userInfo = new UserInfoRes()
                {
                    User = user,
                    Collaborator = collaborator.Models.Single(c => c.Id == user.Id)
                };

                res.Add(userInfo);
            }

            return res;
        }

        public async Task<List<User>> GetSpecialRoles(string nit)
        {
            var res = await client
                .From<User>()
                .Filter(u => u.CompanyNIT, Operator.Equals, nit)
                .Filter(u => u.Role, Operator.NotEqual, UserRoleFactory.GetRoleName(USER_ROLE.LEADER))
                .Filter(u => u.Role, Operator.NotEqual, UserRoleFactory.GetRoleName(USER_ROLE.COLLABORATOR))
                .Get();

            return res.Models;
        }

        public async Task<List<Notification>> GetNotifications(Guid userId)
        {
            var res = await client
                .From<Notification>()
                .Where(n => n.UserId == userId)
                .Get();

            return res.Models;
        }

        public async Task<Notification> AddNotification(Notification notification)
        {
            var res = await client
                .From<Notification>()
                .Insert(notification, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.First();
        }

        public async Task<string> UpdatePhoto(Guid userId, IFormFile file)
        {
            if (file == null)
                return null;

            User user = await GetById(userId);

            if (user == null)
                return null;

            string title = userId.ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.GetTempFileName() + Path.GetExtension(file.FileName);

            using (FileStream stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            string path = Path.Combine(USER_IMAGES_BUCKET, filePath);
            string pathRes = null;

            if (user.Photo == null)
            {
                pathRes = await client.Storage
                    .From(USER_IMAGES_BUCKET)
                    .Upload(path, title, new FileOptions { CacheControl = "3600", Upsert = true });
            }
            else
            {
                pathRes = await client
                    .Storage
                    .From(USER_IMAGES_BUCKET)
                    .Update(path, title, new FileOptions { CacheControl = "3600", Upsert = true });
            }

            if (pathRes == null)
                return null;

            string publicUrl = client
                .Storage
                .From(USER_IMAGES_BUCKET)
                .GetPublicUrl(title);

            if (user.Photo != publicUrl)
            {
                user.Photo = publicUrl;
                await Update(user);
            }

            return publicUrl;
        }

        public async Task<string> GetPhotoUrl(Guid userId)
        {
            User user = await GetById(userId);

            if (user == null || string.IsNullOrEmpty(user.Photo))
                return null;
            else if (!string.IsNullOrEmpty(user.Photo))
                return user.Photo;

            List<FileObject> res = await client
                .Storage
                .From(USER_IMAGES_BUCKET)
                .List() ?? [];

            string userImg = res
                .SingleOrDefault(
                        f => f.Name?.Contains(userId.ToString()) ?? false
                    )?.Name ?? "";

            if (string.IsNullOrEmpty(userImg))
                return null;

            user.Photo = userImg;
            await Update(user);

            return client
                .Storage
                .From(USER_IMAGES_BUCKET)
                .GetPublicUrl(userImg);
        }
    }
}
