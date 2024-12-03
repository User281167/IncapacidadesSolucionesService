using IncapacidadesSoluciones.Models;
using QueryOptions = Supabase.Postgrest.QueryOptions;
using Operator = Supabase.Postgrest.Constants.Operator;
using IncapacidadesSoluciones.Utilities;
using IncapacidadesSoluciones.Dto.Inability;

namespace IncapacidadesSoluciones.Repositories
{
    public class InabilityRepository : IInabilityRepository
    {
        private readonly Supabase.Client client;

        public InabilityRepository(Supabase.Client client)
        {
            this.client = client;
        }

        public async Task<Inability> Insert(Inability inability)
        {
            var res = await client
                .From<Inability>()
                .Insert(inability, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.First();
        }

        public async Task<List<Inability>> GetUserInabilities(Guid id)
        {
            var res = await client
                .From<Inability>()
                .Where(i => i.CollaboratorId == id)
                .Get();

            return res.Models;
        }

        public async Task<List<Inability>> GetNoAccepted(string nit)
        {
            var users = await client
                .From<User>()
                .Select("id")
                .Where(item => item.CompanyNIT == nit)
                .Get();

            var ids = users.Models.Select(u => u.Id).ToList();

            var res = await client
                .From<Inability>()
                .Filter("id_collaborator", Operator.In, ids)
                .Filter("state", Operator.Equals, InabilityStateFactory.GetState(INABILITY_STATE.PENDING))
                .Get();

            return res.Models;
        }

        public async Task<Inability> GetById(Guid id)
        {
            var res = await client
                .From<Inability>()
                .Where(i => i.Id == id)
                .Get();

            return res.Models.FirstOrDefault();
        }

        public async Task<Inability> Update(Inability inability)
        {
            var res = await client
                .From<Inability>()
                .Where(i => i.Id == inability.Id)
                .Update(inability);

            return res.Models.First();
        }

        public async Task<List<InabilityPaymentRes>> GetPaymentReport(string nit)
        {
            var users = await client
                .From<User>()
                .Where(item => item.CompanyNIT == nit)
                .Get();

            var ids = users.Models.Select(u => u.Id).ToList();

            var collaborators = await client
                .From<Collaborator>()
                .Filter("id", Operator.In, ids)
                .Get();

            var inabilities = await client
                .From<Inability>()
                .Filter("id_collaborator", Operator.In, ids)
                .Filter("state", Operator.Equals, InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED))
                .Get();

            var res = new List<InabilityPaymentRes>();

            foreach (var inability in inabilities.Models)
            {
                var user = await client
                    .From<User>()
                    .Where(u => u.Id == inability.CollaboratorId)
                    .Get();

                var userInfo = user.Models.First();

                var collaborator = collaborators.Models.First(c => c.Id == userInfo.Id);

                res.Add(new InabilityPaymentRes()
                {
                    CollaboratorId = userInfo.Id,
                    InabilityId = inability.Id,
                    Name = userInfo.Name,
                    LastName = userInfo.LastName,
                    Cedula = userInfo.Cedula,
                    CompanyPayment = inability.CompanyPayment,
                    HealthEntityPayment = inability.HealthEntityPayment,
                    Pay = inability.Pay,
                    BankAccount = collaborator.BankAccount,
                    BankingEntity = collaborator.BankingEntity,
                });
            }

            return res;
        }

        public async Task<List<Notification>> GetNotifications(Guid id)
        {
            var res = await client
                .From<Notification>()
                .Where(n => n.InabilityId == id)
                .Get();

            return res.Models;
        }

        public async Task<InabilityFile> AddFile(AddFileReq req)
        {
            if (req.File == null)
                return null;

            string title = req.Title + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            var filePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filePath))
            {
                await req.File.CopyToAsync(stream);
            }

            var path = Path.Combine("inability_files", filePath);

            var pathRes = await client.Storage
                .From("inability_files")
                .Upload(path, title);

            if (pathRes == null)
                return null;

            InabilityFile file = new()
            {
                Title = req.Title,
                FileName = title,
                UserId = req.UserId,
                InabilityId = req.InabilityId
            };

            // add to database
            var res = await client
                .From<InabilityFile>()
                .Insert(file, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.FirstOrDefault();
        }
    }
}
