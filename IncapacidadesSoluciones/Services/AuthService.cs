using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Dto.Company;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Utilities.Company;
using IncapacidadesSoluciones.Utilities.Role;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IncapacidadesSoluciones.Services
{
    public class AuthService
    {
        private readonly IUserRepository userRepository;
        private readonly ICompanyRepository companyRepository;

        public AuthService(IUserRepository userRepository, ICompanyRepository companyRepository)
        {
            this.userRepository = userRepository;
            this.companyRepository = companyRepository;
        }

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

        public async Task<AuthRes> RegisterCompany(AuthCompanyReq req)
        {
            if (await userRepository.UserExists(req.Leader.Email, req.Leader.Cedula))
                return new AuthRes { ErrorMessage = "Ya existe un usuario con ese correo o cédula registrado" };
            if (await companyRepository.CompanyExists(req.Company.Nit))
                return new AuthRes { ErrorMessage = "Ya existe una empresa con ese nit registrado" };
            if (!CompanyTypeFactory.IsValid(req.Company.Type))
                return new AuthRes { ErrorMessage = "Tipo de empresa inválido" };
            if (!CompanySectorFactory.IsValid(req.Company.Sector))
                return new AuthRes { ErrorMessage = "Sector de la empresa inválido" };

            try
            {
                var user = await userRepository.SignUp(req.Leader.Email, req.Leader.Password);

                if (user == null)
                    return new AuthRes { ErrorMessage = "Error al registrar el usuario y compañia" };

                Company company = new Company
                {
                    Nit = req.Company.Nit,
                    Name = req.Company.Name,
                    Description = req.Company.Description,
                    Email = req.Company.Email,
                    Founded = req.Company.Founded ?? null,
                    Address = req.Company.Address,
                    Type = req.Company.Type.ToLower(),
                    Sector = req.Company.Sector.ToLower(),
                    LeaderId = user.Id
                };

                var companyRes = await companyRepository.Insert(company);
                
                user.Name = req.Leader.Name;
                user.LastName = req.Leader.LastName;
                user.Phone = req.Leader.Phone;
                user.Cedula = req.Leader.Cedula;
                user.CompanyNIT = companyRes?.Nit ?? "";
                user.Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER);

                var res = await userRepository.Update(user);

                if (res == null)
                    return new AuthRes { ErrorMessage = "Error no se pudo crear el usuario lider" };

                return new AuthRes
                {
                    Token = CreateToken(res, USER_ROLE.LEADER),
                    User = res,
                    ErrorMessage = companyRes == null ? "Error al registrar la compañia" : ""
                };
            }
            catch (Exception ex)
            {
                return new AuthRes
                {
                    ErrorMessage = "Error interno al registrar el usuario y compañia"
                };
            }
        }

        private async Task<Company> RegisterCompany(CompanyReq req, Guid liderId)
        {
            if (!CompanyTypeFactory.IsValid(req.Type))
                return null;
            if (!CompanySectorFactory.IsValid(req.Sector))
                return null;

            var company = new Company
            {
                Nit = req.Nit,
                Name = req.Name,
                Description = req.Description,
                Email = req.Email,
                Founded = req.Founded,
                Address = req.Address,
                Type = req.Type.ToLower(),
                Sector = req.Sector.ToLower(),
                LeaderId = liderId
            };

            var res = await companyRepository.Insert(company);
            return res;
        }

        private async Task<AuthRes> RegisterUser(AuthUserReq req, USER_ROLE role)
        {
            
            if (await userRepository.UserExists(req.Email, req.Cedula))
                return new AuthRes { ErrorMessage = "Ya existe un usuario con ese correo o cédula registrado" };
            
            var user = await userRepository.SignUp(req.Email, req.Password);

            if (user == null)
                return new AuthRes { ErrorMessage = "Error al registrar el usuario" };

            user.Name = req.Name;
            user.LastName = req.LastName;
            user.Phone = req.Phone;
            user.Cedula = req.Cedula;
            user.Role = UserRoleFactory.GetRoleName(role);

            var res = await userRepository.Update(user);
            
            if (res == null)
                return new AuthRes { ErrorMessage = "Error no se pudo crear el usuario" };

            return new AuthRes
            {
                Token = CreateToken(res, role),
                User = res,
                ErrorMessage = ""
            };
        }
    }
}
