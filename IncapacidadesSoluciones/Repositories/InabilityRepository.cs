﻿using IncapacidadesSoluciones.Models;
using QueryOptions = Supabase.Postgrest.QueryOptions;
using Operator = Supabase.Postgrest.Constants.Operator;
using IncapacidadesSoluciones.Utilities;

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
                .Where(i => i.IdCollaborator == id)
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
    }
}
