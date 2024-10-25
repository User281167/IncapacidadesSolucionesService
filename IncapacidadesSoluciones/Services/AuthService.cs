using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Dto.Company;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Utilities.Company;
using IncapacidadesSoluciones.Utilities.Role;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using QueryOptions = Supabase.Postgrest.QueryOptions;

namespace IncapacidadesSoluciones.Services
{
    public class AuthService
    {
        private string CreateToken(User user, USER_ROLE role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, UserRoleFactory.GetRoleName(role)),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Boolean> UserExists(string email, string cedula, Supabase.Client client)
        {
            var res = await client
                .From<User>()
                .Where(user => user.Email == email || user.Cedula == cedula)
                .Single();

            return res != null;
        }

        public async Task<Boolean> CompanyExists(string nit, Supabase.Client client)
        {
            var res = await client
                            .From<User>()
                            .Where(user => user.CompanyNIT == nit)
                            .Single();

            return res != null;
        }

        public async Task<AuthRes> RegisterCompany(AuthCompanyReq req, Supabase.Client client)
        {
            if (await UserExists(req.User.Email, req.User.Cedula, client))
                return new AuthRes { ErrorMessage = "Ya existe un usuario con ese correo o cédula registrado" };
            if (await CompanyExists(req.Company.Type, client))
                return new AuthRes { ErrorMessage = "Ya existe una empresa con ese nit registrado" };
            if (!CompanyTypeFactory.IsValid(req.Company.Type))
                return new AuthRes { ErrorMessage = "Tipo de empresa inválido" };
            if (!CompanySectorFactory.IsValid(req.Company.Sector))
                return new AuthRes { ErrorMessage = "Sector de la empresa inválido" };

            try
            {
                var session = await client.Auth.SignUp(req.User.Email, req.User.Password);

                // trigger event create user
                var user = await client
                    .From<User>()
                    .Where(u => u.Email == req.User.Email)
                    .Single();

                if (user == null)
                    return new AuthRes { ErrorMessage = "Error al registrar el usuario y compañia" };

                Company company = new Company
                {
                    NIT = req.Company.Type,
                    Name = req.Company.Type,
                    Description = req.Company.Type,
                    Email = req.Company.Type,
                    CreatedAt = req.Company.CreatedAt,
                    Address = req.Company.Type,
                    Type = req.Company.Type.ToLower(),
                    Sector = req.Company.Sector.ToLower(),
                    LeaderId = user.Id
                };

                var companyRes = await client
                  .From<Company>()
                  .Insert(company, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

                user.Name = req.User.Name;
                user.LastName = req.User.LastName;
                user.Phone = req.User.Phone;
                user.Cedula = req.User.Cedula;
                user.CompanyNIT = companyRes?.Models.First().NIT ?? "";
                user.Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER);

                var res = await client
                    .From<User>()
                    .Where(u => u.Email == req.User.Email)
                    .Update(user);

                if (res.Models.Count == 0)
                    return new AuthRes { ErrorMessage = "Error no se pudo crear el usuario lider" };

                return new AuthRes
                {
                    Token = CreateToken(res.Models.First(), USER_ROLE.LEADER),
                    User = res.Models.First(),
                    ErrorMessage = company == null ? "Error al registrar la compañia" : ""
                };
            }
            catch (Exception ex)
            {
                return new AuthRes
                {
                    ErrorMessage = "Error al registrar el usuario y compañia"
                };
            }
        }

        private async Task<Company> RegisterCompany(CompanyReq req, Guid liderId, Supabase.Client client)
        {
            if (!CompanyTypeFactory.IsValid(req.Type))
                return null;
            if (!CompanySectorFactory.IsValid(req.Sector))
                return null;

            var company = new Company
            {
                NIT = req.Nit,
                Name = req.Name,
                Description = req.Description,
                Email = req.Email,
                CreatedAt = req.CreatedAt,
                Address = req.Address,
                Type = req.Type.ToLower(),
                Sector = req.Sector.ToLower(),
                LeaderId = liderId
            };

            var res = await client.
                From<Company>()
                .Insert(company, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            return res.Models.First();
        }

        private async Task<AuthRes> RegisterUser(AuthUserReq req, USER_ROLE role, Supabase.Client client)
        {
            if (await UserExists(req.Email, req.Cedula, client))
                return new AuthRes { ErrorMessage = "Ya existe un usuario con ese correo o cédula registrado" };

            var session = await client.Auth.SignUp(req.Email, req.Password);

            if (session == null)
                return new AuthRes { ErrorMessage = "Error al registrar el usuario" };

            // trigger event create user after sign up
            var user = await client
                .From<User>()
                .Where(u => u.Email == req.Email)
                .Single();

            if (user == null)
                return new AuthRes { ErrorMessage = "Error al registrar el usuario" };

            user.Name = req.Name;
            user.LastName = req.LastName;
            user.Phone = req.Phone;
            user.Cedula = req.Cedula;
            user.Role = UserRoleFactory.GetRoleName(role);

            var res = await client
                .From<User>()
                .Where(u => u.Email == req.Email)
                .Update(user);

            if (res.Models.Count == 0)
                return new AuthRes { ErrorMessage = "Error no se pudo crear el usuario" };

            return new AuthRes
            {
                Token = CreateToken(res.Models.First(), role),
                User = res.Models.First(),
                ErrorMessage = ""
            };
        }
    }
}
