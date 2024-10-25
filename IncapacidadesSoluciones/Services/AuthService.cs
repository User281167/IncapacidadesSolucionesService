using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IncapacidadesSoluciones.Services
{
    public class AuthService
    {
        private string CreateToken(User user, ROLE role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, RoleFactory.GetRoleName(role)),
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

        public async Task<AuthRes> RegisterCompany(AuthCompanyReq credentials, Supabase.Client client)
        {
            if (await UserExists(credentials.LeaderEmail, credentials.LeaderCedula, client))
                return new AuthRes { ErrorMessage = "Ya existe un usuario con ese correo o cédula" };

            try
            {
                var session = await client.Auth.SignUp(credentials.LeaderEmail, credentials.LeaderPassword);

                // trigger event create user
                var user = await client
                    .From<User>()
                    .Where(u => u.Email == credentials.LeaderEmail)
                    .Single();

                if (user == null)
                    return new AuthRes { ErrorMessage = "Error al registrar el usuario y compañia" };

                user.Name = credentials.LeaderName;
                user.LastName = credentials.LeaderLastName;
                user.Phone = credentials.LeaderPhone;
                user.Cedula = credentials.LeaderCedula;
                user.CompanyNIT = credentials.Nit;
                user.Role = RoleFactory.GetRoleName(ROLE.LIDER);

                var res = await client
                    .From<User>()
                    .Where(u => u.Email == credentials.LeaderEmail)
                    .Update(user);

                if (res.Models.Count == 0)
                    return new AuthRes { ErrorMessage = "Error no se pudo crear el usuario lider "};

                return new AuthRes
                {
                    Token = CreateToken(res.Models.First(), ROLE.LIDER),
                    UserData = res.Models.First()
                };
            }
            catch (Exception ex)
            {
                return new AuthRes
                {
                    ErrorMessage = "Error al registrar el usuario y compañia -> " + ex.Message
                };
            }
        }
    }
}
