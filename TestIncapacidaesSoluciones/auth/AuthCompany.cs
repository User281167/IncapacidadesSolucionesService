﻿using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Dto.Company;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using IncapacidadesSoluciones.Utilities.Company;
using IncapacidadesSoluciones.Utilities.Role;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestIncapacidadesSoluciones.auth;

namespace TestIncapacidaesSoluciones
{
    public class AuthCompany
    {
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<ICompanyRepository> companyRepository;
        private readonly Mock<IAccessCodeRepository> accessCodeRepository;
        private readonly AuthService authService;
        private readonly AuthController authController;
        private readonly new Mock<IEnvironmentWrapper> environmentMock;

        private readonly CompanyReq companyTets = new CompanyReq
        {
            Id = new Guid(),
            Nit = "test",
            Name = "test",
            Description = "test",
            Email = "test@test.com",
            Founded = DateOnly.FromDateTime(DateTime.Now),
            Address = "test",
            Type = "test",
            Sector = "test"
        };

        private readonly AuthCompanyReq authCompanyReq = new()
        {
            Nit = "test",
            Name = "test",
            Description = "test",
            Email = "test@test.com",
            Founded = DateOnly.FromDateTime(DateTime.Now),
            Address = "test",
            Type = "test",
            Sector = "test",
            LeaderName = "test",
            LeaderLastName = "test",
            LeaderCedula = "test",
            LeaderPhone = "test",
            LeaderEmail = "test@test.com",
            Password = "test"
        };

        public AuthCompany()
        {
            userRepository = new Mock<IUserRepository>();
            companyRepository = new Mock<ICompanyRepository>();
            accessCodeRepository = new Mock<IAccessCodeRepository>();

            authService = new AuthService(userRepository.Object, companyRepository.Object, accessCodeRepository.Object);
            authController = new AuthController(authService);
            environmentMock = new Mock<IEnvironmentWrapper>();
        }

        [Fact]
        public async void RegisterCompanyAndLeader_Req_Null()
        {
            var res = await authController.RegisterCompany(null);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("La información no puede ser nula.", bad.Value);
        }

        [Fact]
        public async void RegisterCompanyAndLeader_UserExists_BadRequest()
        {
            // Arrange
            var req = authCompanyReq;

            userRepository.Setup(repo => repo.UserExists(req.LeaderEmail, req.LeaderCedula)).ReturnsAsync(true);

            // Act
            var res = await authController.RegisterCompany(req);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("Ya existe un usuario con ese correo o cédula registrado", bad.Value);
        }

        [Fact]
        public async void RegisterCompanyAndLeader_CompanyExists_BadRequest()
        {
            // Arrange
            var req = authCompanyReq;

            companyRepository.Setup(repo => repo.CompanyExists(req.Nit)).ReturnsAsync(true);

            // Act
            var res = await authController.RegisterCompany(req);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("Ya existe una empresa con ese nit registrado", bad.Value);
        }

        [Fact]
        public async void RegisterCompanyAndLeader_CompanyType_BadRequest()
        {
            // Arrange
            var req = authCompanyReq;

            // Act
            var res = await authController.RegisterCompany(req);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("Tipo de empresa inválido", bad.Value);
        }

        [Fact]
        public async void RegisterCompanyAndLeader_CompanySector_BadRequest()
        {
            // Arrange
            var req = authCompanyReq;
            req.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);

            // Act
            var res = await authController.RegisterCompany(req);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("Sector de la empresa inválido", bad.Value);
        }

        [Fact]
        public async void RegisterCompanyAndLeader_SignUp_BadRequest()
        {
            // Arrange
            var req = authCompanyReq;
            req.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            req.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            // Act
            var res = await authController.RegisterCompany(req);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("Error al registrar el usuario y compañia", bad.Value);
        }

        [Fact]
        public async void RegisterCompanyAndLeader_Leader_BadRequest()
        {
            // Arrange
            var req = authCompanyReq;
            req.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            req.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            userRepository.Setup(repo => repo.SignUp(req.LeaderEmail, req.Password)).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.UpdateByEmail(new User())).ReturnsAsync(new User());

            // Act
            var res = await authController.RegisterCompany(req);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("Error no se pudo crear el usuario lider", bad.Value);
        }

        [Fact]
        public async void RegisterCompanyAndLeader_Company_OkRequest()
        {
            // Arrange
            Environment.SetEnvironmentVariable("JWT_KEY", EnvironmentWrapper.JWT_KEY_TEST);

            var req = authCompanyReq;
            req.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            req.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var user = new User
            {
                Id = new Guid(),
                Name = req.LeaderName,
                LastName = req.LeaderLastName,
                Phone = req.LeaderPhone,
                Cedula = req.LeaderCedula,
                Email = req.LeaderEmail
            };

            environmentMock.Setup(env => env.GetEnvironmentVariable("JWT_KEY")).Returns(EnvironmentWrapper.JWT_KEY_TEST);
            userRepository.Setup(repo => repo.SignUp(req.LeaderEmail, req.Password)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.UpdateByEmail(It.IsAny<User>())).ReturnsAsync(user);

            // Act
            var res = await authController.RegisterCompany(req);

            // Assert
            var bad = Assert.IsType<OkObjectResult>(res);
            var authRes = bad.Value as AuthRes;

            Assert.NotNull(authRes);
            Assert.Equal("Error al registrar la compañia", authRes.ErrorMessage);
            Assert.Equal(authRes.User.Id, user.Id);
            Assert.Equal(authRes.User.Name, user.Name);
            Assert.NotNull(authRes.Token);
            Assert.NotEmpty(authRes.Token);
            Assert.Equal(UserRoleFactory.GetRoleName(USER_ROLE.LEADER), authRes.User.Role);
        }

        [Fact]
        public async void RegisterCompanyAndLeader_Company_Ok()
        {
            // Arrange
            Environment.SetEnvironmentVariable("JWT_KEY", EnvironmentWrapper.JWT_KEY_TEST);

            var req = authCompanyReq;
            req.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            req.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var user = new User
            {
                Id = new Guid(),
                Name = req.LeaderName,
                LastName = req.LeaderLastName,
                Phone = req.LeaderPhone,
                Cedula = req.LeaderCedula,
                Email = req.LeaderEmail
            };

            environmentMock.Setup(env => env.GetEnvironmentVariable("JWT_KEY")).Returns(EnvironmentWrapper.JWT_KEY_TEST);
            userRepository.Setup(repo => repo.SignUp(req.LeaderEmail, req.Password)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.UpdateByEmail(It.IsAny<User>())).ReturnsAsync(user);
            companyRepository.Setup(repo => repo.Insert(It.IsAny<Company>())).ReturnsAsync(new Company());

            // Act
            var res = await authController.RegisterCompany(req);

            // Assert
            var bad = Assert.IsType<OkObjectResult>(res);
            var authRes = bad.Value as AuthRes;

            Assert.NotNull(authRes);
            Assert.Equal("", authRes.ErrorMessage);
            Assert.Equal(authRes.User.Id, user.Id);
            Assert.Equal(authRes.User.Name, user.Name);
            Assert.NotNull(authRes.Token);
            Assert.NotEmpty(authRes.Token);
            Assert.Equal(UserRoleFactory.GetRoleName(USER_ROLE.LEADER), authRes.User.Role);
        }

        [Fact]
        public async void UpdateCompany_ErrorNit()
        {
            User leader = new User
            {
                CompanyNIT = "test",
                Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER)
            };

            Guid leaderId = new("00000000-0000-0000-0000-000000000001");

            var company = companyTets;
            company.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            company.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var req = new CompanyReq
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                Nit = "nit123",
                Name = company.Name,
                Description = company.Description,
                Email = company.Email,
                Founded = company.Founded,
                Address = company.Address,
                Type = company.Type,
                Sector = company.Sector
            };

            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(leader);
            companyRepository.Setup(
                repo => repo.GetCompany(It.IsAny<Guid>())
                ).ReturnsAsync(new Company() { LeaderId = leaderId });
            companyRepository.Setup(repo => repo.CompanyExists(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var res = await authController.UpdateCompany(leaderId, req);

            // Assert
            Assert.NotNull(res);
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void UpdateCompany_LeaderHasNoPermissions()
        {
            User leader = new User
            {
                CompanyNIT = "test",
                Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER)
            };

            Guid leaderId = new("00000000-0000-0000-0000-000000000001");

            var company = companyTets;
            company.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            company.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var req = new CompanyReq
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                Nit = "nit123",
                Name = company.Name,
                Description = company.Description,
                Email = company.Email,
                Founded = company.Founded,
                Address = company.Address,
                Type = company.Type,
                Sector = company.Sector
            };

            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(leader);
            companyRepository.Setup(
                repo => repo.GetCompany(It.IsAny<Guid>())
                ).ReturnsAsync(new Company());
            companyRepository.Setup(repo => repo.CompanyExists(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var res = await authController.UpdateCompany(leaderId, req);

            // Assert
            Assert.NotNull(res);
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void UpdateCompany_Ok()
        {
            User leader = new User
            {
                CompanyNIT = "test",
                Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER)
            };

            Guid leaderId = new("00000000-0000-0000-0000-000000000001");

            var company = companyTets;
            company.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            company.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var req = new CompanyReq
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                Nit = "nit123",
                Name = company.Name,
                Description = company.Description,
                Email = company.Email,
                Founded = company.Founded,
                Address = company.Address,
                Type = company.Type,
                Sector = company.Sector
            };

            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(leader);
            companyRepository.Setup(
                repo => repo.GetCompany(It.IsAny<Guid>())
                ).ReturnsAsync(new Company() { LeaderId = leaderId });
            companyRepository.Setup(repo => repo.CompanyExists(It.IsAny<string>())).ReturnsAsync(false);
            companyRepository.Setup(repo => repo.Update(It.IsAny<Company>())).ReturnsAsync(new Company());

            // Act
            var res = await authController.UpdateCompany(leaderId, req);

            // Assert
            Assert.NotNull(res);
            Assert.IsType<OkResult>(res);
        }
    }
}
