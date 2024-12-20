﻿using IncapacidadesSoluciones.Dto;
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

        public AuthService(IUserRepository userRepository, ICompanyRepository companyRepository, IAccessCodeRepository loginCodeRepository)
        {
            this.userRepository = userRepository;
            this.companyRepository = companyRepository;
            this.accessCodeRepository = loginCodeRepository;
        }

        public async Task<AuthRes> RegisterCompany(AuthCompanyReq req)
        {
            if (await userRepository.UserExists(req.LeaderEmail, req.LeaderCedula))
                return new AuthRes { ErrorMessage = "Ya existe un usuario con ese correo o cédula registrado" };
            if (await companyRepository.CompanyExists(req.Nit))
                return new AuthRes { ErrorMessage = "Ya existe una empresa con ese nit registrado" };
            if (!CompanyTypeFactory.IsValid(req.Type))
                return new AuthRes { ErrorMessage = "Tipo de empresa inválido" };
            if (!CompanySectorFactory.IsValid(req.Sector))
                return new AuthRes { ErrorMessage = "Sector de la empresa inválido" };

            try
            {
                var user = await userRepository.SignUp(req.LeaderEmail, req.Password);

                if (user == null)
                    return new AuthRes { ErrorMessage = "Error al registrar el usuario y compañia" };

                Company company = new Company
                {
                    Nit = req.Nit,
                    Name = req.Name,
                    Description = req.Description ?? "",
                    Email = req.Email,
                    Founded = req.Founded ?? null,
                    Address = req.Address ?? "",
                    Type = req.Type.ToLower(),
                    Sector = req.Sector.ToLower(),
                    LeaderId = user.Id
                };

                var companyRes = await companyRepository.Insert(company);

                user.Name = req.LeaderName;
                user.LastName = req.LeaderLastName;
                user.Phone = req.LeaderPhone ?? "";
                user.Cedula = req.LeaderCedula;
                user.CompanyNIT = companyRes?.Nit ?? "";
                user.Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER);

                var res = await userRepository.UpdateByEmail(user);

                if (res == null)
                    return new AuthRes { ErrorMessage = "Error no se pudo crear el usuario lider" };

                return new AuthRes
                {
                    Token = JWT.CreateToken(res, USER_ROLE.LEADER),
                    User = res,
                    ErrorMessage = companyRes == null ? "Error al registrar la compañia" : ""
                };
            }
            catch (Exception ex)
            {
                return new AuthRes
                {
                    ErrorMessage = "Error interno al registrar el usuario y compañia " + ex
                };
            }
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

            System.Console.WriteLine(company.LeaderId);
            System.Console.WriteLine(leaderId);

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
            if (req.AccessCode == null || req.AccessCode == "")
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
            user.CompanyNIT = code.NIT;

            var res = await userRepository.UpdateByEmail(user);

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
                NIT = company.Nit,
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

            code.NIT = company.Nit;
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

        public async Task<ApiRes<User>> CreateRole(AuthRoleReq req)
        {
            USER_ROLE role = UserRoleFactory.GetRole(req.Role);

            if (role == USER_ROLE.NOT_FOUND)
                return new ApiRes<User> { Message = $"No se puede crear un usuario con credenciales de {req.Role}." };
            else if (role == USER_ROLE.LEADER)
                return new ApiRes<User> { Message = "No se puede crear un usuario con credenciales de LIDER." };
            else if (role == USER_ROLE.COLLABORATOR)
                return new ApiRes<User> { Message = "No se puede crear un usuario con credenciales de COLABORADOR." };

            // check leader and get company nit
            var leader = await userRepository.GetById(req.LeaderId);

            if (leader == null)
                return new ApiRes<User> { Message = "Compruebe que tengas las credenciales necesarias para asignar roles." };
            else if (await userRepository.UserExists(req.Email, req.Cedula))
                return new ApiRes<User> { Message = "Ya existe un usuario con ese correo o cédula registrado." };

            var user = await userRepository.SignUp(req.Email, req.Password);

            if (user == null)
                return new ApiRes<User> { Message = "Error al registrar el usuario" };

            user.Name = req.Name;
            user.LastName = req.LastName;
            user.Email = req.Email.ToLower();
            user.Cedula = req.Cedula;
            user.Phone = req.Phone;
            user.Role = UserRoleFactory.GetRoleName(role);
            user.CompanyNIT = leader.CompanyNIT;

            var res = await userRepository.UpdateByEmail(user);

            if (res == null)
                return new ApiRes<User> { Message = "Error al registrar y actualizar el usuario" };

            return new ApiRes<User>
            {
                Success = true,
                Message = "Usuario creado con éxito.",
                Data = res
            };
        }

        public async Task<ApiRes<User>> UpdateRole(AuthRoleReq req)
        {
            if (req.UserId == null)
                return new ApiRes<User> { Message = "El campo Id es obligatorio." };

            var role = UserRoleFactory.GetRole(req.Role);

            if (role == USER_ROLE.NOT_FOUND)
                return new ApiRes<User> { Message = $"No se puede crear un usuario con credenciales de {req.Role}." };
            else if (role == USER_ROLE.LEADER)
                return new ApiRes<User> { Message = "No se puede crear un usuario con credenciales de LIDER." };
            else if (role == USER_ROLE.COLLABORATOR)
                return new ApiRes<User> { Message = "No se puede crear un usuario con credenciales de COLABORADOR." };

            // check leader and get company nit
            var leader = await userRepository.GetById(req.LeaderId);

            if (leader == null)
                return new ApiRes<User>
                {
                    Message = "Compruebe que tengas las credenciales necesarias para asignar roles."
                };

            var user = await userRepository.GetById(req.UserId ?? Guid.Empty);
            var UserExists = await userRepository.GetByEmailOrCedula(req.Email, req.Cedula);

            if (user == null)
                return new ApiRes<User> { Message = "No se pudo encontrar el usuario." };
            else if (UserExists != null && UserExists.Id != user.Id)
                return new ApiRes<User>
                {
                    Message = "Ya existe un usuario con ese correo o cédula registrado."
                };

            user.Name = req.Name;
            user.LastName = req.LastName;
            user.Email = req.Email.ToLower();
            user.Cedula = req.Cedula;
            user.Phone = req.Phone;
            user.Role = UserRoleFactory.GetRoleName(role);
            user.CompanyNIT = leader.CompanyNIT;

            var res = await userRepository.Update(user);

            if (res == null)
                return new ApiRes<User> { Message = "Error al actualizar el usuario" };

            return new ApiRes<User>
            {
                Success = true,
                Message = "Usuario actualizado con éxito.",
                Data = res
            };
        }

        public async Task<ApiRes<bool>> DeleteRole(DeleteAuthReq req)
        {
            var leader = await userRepository.GetById(req.LeaderId);

            if (leader == null)
                return new ApiRes<bool> { Message = "No se pudo encontrar el líder." };

            var user = await userRepository.GetById(req.UserId);

            if (user == null)
                return new ApiRes<bool> { Message = "No se pudo encontrar el usuario." };
            else if (leader.Id == user.Id)
                return new ApiRes<bool> { Message = "No se puede eliminar el usuario que es el líder." };
            else if (leader.CompanyNIT != user.CompanyNIT)
                return new ApiRes<bool>
                {
                    Message = "No se puede eliminar el usuario que no pertenece a la empresa del líder."
                };

            await userRepository.Delete(user.Id);

            return new ApiRes<bool>
            {
                Success = true,
                Message = "Usuario eliminado con éxito.",
                Data = true
            };
        }
    }
}
