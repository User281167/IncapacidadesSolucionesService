using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Dto.Company;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Utilities.Company;
using IncapacidadesSoluciones.Utilities.Role;

namespace IncapacidadesSoluciones.Services
{
    public class AuthService
    {
        private readonly IUserRepository userRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IAccessCodeRepository accessCodeRepository;

        public AuthService(IUserRepository userRepository, ICompanyRepository companyRepository, IAccessCodeRepository accessCodeRepository)
        {
            this.userRepository = userRepository;
            this.companyRepository = companyRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public async Task<AuthRes> RegisterCompany(AuthCompanyReq req)
        {
            if (await userRepository.UserExists(req.LeaderEmail, req.LeaderCedula))
                return new AuthRes { ErrorMessage = "Ya existe un usuario con ese correo o cédula registrado" };
            else if (await companyRepository.CompanyExists(req.Nit))
                return new AuthRes { ErrorMessage = "Ya existe una empresa con ese nit registrado" };
            else if (!CompanyTypeFactory.IsValid(req.Type))
                return new AuthRes { ErrorMessage = "Tipo de empresa inválido" };
            else if (!CompanySectorFactory.IsValid(req.Sector))
                return new AuthRes { ErrorMessage = "Sector de la empresa inválido" };

            try
            {
                User user = await userRepository.SignUp(req.LeaderEmail, req.Password);

                if (user == null)
                    return new AuthRes { ErrorMessage = "Error al registrar el usuario y empresa." };

                Company company = new()
                {
                    Nit = req.Nit,
                    Name = req.Name,
                    Description = req.Description,
                    Email = req.Email,
                    Founded = req.Founded,
                    Address = req.Address,
                    Type = req.Type.ToLower(),
                    Sector = req.Sector.ToLower(),
                    LeaderId = user.Id
                };

                user.Name = req.LeaderName;
                user.LastName = req.LeaderLastName;
                user.Phone = req.LeaderPhone;
                user.Cedula = req.LeaderCedula;
                user.CompanyNIT = company.Nit;
                user.Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER);

                Task<Company> insertingCompany = companyRepository.Insert(company);
                Task<User> updatingUser = userRepository.UpdateByEmail(user);

                User updateUser = await updatingUser;
                Company companyInsert = await insertingCompany;

                if (updateUser == null)
                    return new AuthRes { ErrorMessage = "Error no se pudo crear el usuario líder" };

                return new AuthRes
                {
                    Token = JWT.CreateToken(updateUser, USER_ROLE.LEADER),
                    User = updateUser,
                    ErrorMessage = companyInsert == null ? "Error al registrar la empresa" : ""
                };
            }
            catch (Exception ex)
            {
                return new AuthRes
                {
                    ErrorMessage = "Error interno al registrar el usuario y empresa."
                };
            }
        }

        public async Task<ApiRes<Company>> GetCompany(string nit)
        {
            var company = await companyRepository.GetCompanyByNit(nit);

            if (company == null)
                return new ApiRes<Company> { Message = "No se pudo encontrar la empresa." };

            return new ApiRes<Company>
            {
                Success = true,
                Message = "Empresa obtenida con éxito.",
                Data = company
            };
        }

        public async Task<string> UpdateCompany(Guid leaderId, CompanyReq req)
        {
            if (!CompanyTypeFactory.IsValid(req.Type))
                return "Tipo de empresa inválido.";
            else if (!CompanySectorFactory.IsValid(req.Sector))
                return "Sector de la empresa inválido.";
            else if (req.Id == Guid.Empty)
                return "Id de la empresa no puede ser vacío.";

            var leader = await userRepository.GetById(leaderId);

            if (leader == null)
                return "No se pudo encontrar el líder.";
            else if (leader.Role != UserRoleFactory.GetRoleName(USER_ROLE.LEADER))
                return "No se puede actualizar la empresa.";

            var company = await companyRepository.GetCompany(req.Id);

            if (company == null)
                return "No se pudo encontrar la empresa.";
            else if (company.LeaderId != leaderId)
                return "No se puede actualizar la empresa no tienes permisos para hacerlo.";
            else if (company.Nit != req.Nit && await companyRepository.CompanyExists(req.Nit))
                return "Ya existe una empresa con ese nit registrado.";

            company.Name = req.Name;
            company.Description = req.Description;
            company.Email = req.Email;
            company.Founded = req.Founded;
            company.Address = req.Address;
            company.Type = req.Type.ToLower();
            company.Sector = req.Sector.ToLower();
            company.Nit = req.Nit;

            var res = await companyRepository.Update(company);
            return res == null ? "Error al actualizar la empresa." : "";
        }

        public async Task<AuthRes> RegisterUser(AuthUserReq req, USER_ROLE role)
        {
            if (string.IsNullOrEmpty(req.AccessCode))
                return new AuthRes { ErrorMessage = "Código de acceso requerido" };

            var code = await accessCodeRepository.GetByCode(req.AccessCode);

            if (code == null)
                return new AuthRes { ErrorMessage = "Código de acceso inválido" };
            else if (code.ExpirationDate != null && code.ExpirationDate < DateOnly.FromDateTime(DateTime.Now))
                return new AuthRes { ErrorMessage = "Código de acceso expirado" };
            else if (await userRepository.UserExists(req.Email, req.Cedula))
                return new AuthRes { ErrorMessage = "Ya existe un usuario con ese correo o cédula registrado" };

            var user = await userRepository.SignUp(req.Email, req.Password);

            if (user == null)
                return new AuthRes { ErrorMessage = "Error al registrar el usuario" };

            user.Name = req.Name;
            user.LastName = req.LastName;
            user.Phone = req.Phone;
            user.Cedula = req.Cedula;
            user.Role = UserRoleFactory.GetRoleName(role);
            user.CompanyNIT = code.CompanyNit;

            var res = await userRepository.UpdateByEmail(user);

            if (role == USER_ROLE.COLLABORATOR)
            {
                var collaborator = await userRepository.CreateCollaborator(res.Id);

                if (collaborator == null)
                    return new AuthRes { ErrorMessage = "Error al registrar el colaborador" };
            }

            if (res == null)
                return new AuthRes { ErrorMessage = "Error no se pudo crear el usuario" };

            return new AuthRes
            {
                Token = JWT.CreateToken(res, role),
                User = res,
                ErrorMessage = ""
            };
        }

        public async Task<ApiRes<AccessCode>> CreateAccessCode(AuthAccessCodeReq req)
        {
            var company = await companyRepository.GetCompany(req.CompanyId);

            if (company == null)
                return new ApiRes<AccessCode> { Success = false, Message = "No se pudo encontrar la empresa." };

            var code = new AccessCode
            {
                CompanyNit = company.Nit,
                ExpirationDate = req.ExpirationDate,
                Code = AccessCode.GenerateCode(company.Name)
            };

            var res = await accessCodeRepository.Insert(code);
            return new ApiRes<AccessCode> { Data = res, Success = true, Message = "Código de acceso generado con exito." };
        }

        public async Task<ApiRes<AccessCode>> UpdateAccessCode(AuthAccessCodeReq req)
        {
            var company = await companyRepository.GetCompany(req.CompanyId);
            var code = await accessCodeRepository.GetById(req.Id);

            if (company == null)
                return new ApiRes<AccessCode> { Success = false, Message = "No se pudo encontrar la empresa." };
            else if (code == null)
                return await CreateAccessCode(req);

            code.CompanyNit = company.Nit;
            code.ExpirationDate = req.ExpirationDate;
            code.Code = AccessCode.GenerateCode(company.Name);

            var res = await accessCodeRepository.Update(code);
            return new ApiRes<AccessCode> { Data = res, Success = true, Message = "Código de acceso actualizado con exito." };
        }

        public async Task<AuthRes> Login(AuthLoginReq req)
        {
            User user;

            try
            {
                user = await userRepository.SignIn(req.Email, req.Password);
            }
            catch (Supabase.Gotrue.Exceptions.GotrueException ex)
            {
                if (ex.StatusCode == 400)
                    return new AuthRes { ErrorMessage = "Credenciales incorrectas." };

                return new AuthRes { ErrorMessage = "Error al iniciar sesión intentalo más tarde." };
            }

            if (user == null)
                return new AuthRes { ErrorMessage = "Error al iniciar sesión comprueba tus credenciales." };

            var role = UserRoleFactory.GetRole(user.Role);

            return new AuthRes
            {
                User = user,
                Token = JWT.CreateToken(user, role)
            };
        }
        public bool ValidateToken(string token)
        {
            return JWT.ValidateToken(token);
        }
    }
}
