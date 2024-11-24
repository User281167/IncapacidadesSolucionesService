using IncapacidadesSoluciones.Controllers;
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

        private readonly AuthUserReq userTest = new AuthUserReq
        {
            AccessCode = "test",
            Name = "test",
            LastName = "test",
            Phone = "test",
            Cedula = "test",
            Email = "test@test.com",
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
            var req = new AuthCompanyReq {
                Company = companyTets,
                Leader = userTest
            };

            userRepository.Setup(repo => repo.UserExists(req.Leader.Email, req.Leader.Cedula)).ReturnsAsync(true);

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
            var req = new AuthCompanyReq
            {
                Company = companyTets,
                Leader = userTest
            };

            companyRepository.Setup(repo => repo.CompanyExists(req.Company.Nit)).ReturnsAsync(true);

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
            var req = new AuthCompanyReq
            {
                Company = companyTets,
                Leader = userTest
            };

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
            var company = companyTets;
            company.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);

            var req = new AuthCompanyReq
            {
                Company = company,
                Leader = userTest
            };

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
            var company = companyTets;
            company.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            company.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var req = new AuthCompanyReq
            {
                Company = company,
                Leader = userTest
            };

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
            var company = companyTets;
            company.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            company.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var req = new AuthCompanyReq
            {
                Company = company,
                Leader = userTest
            };

            userRepository.Setup(repo => repo.SignUp(req.Leader.Email, req.Leader.Password)).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.Update(new User())).ReturnsAsync(new User());

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

            var company = companyTets;
            company.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            company.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var req = new AuthCompanyReq
            {
                Company = company,
                Leader = userTest
            };

            var user = new User
            {
                Id = new Guid(),
                Name = req.Leader.Name,
                LastName = req.Leader.LastName,
                Phone = req.Leader.Phone,
                Cedula = req.Leader.Cedula,
                Email = req.Leader.Email
            };

            environmentMock.Setup(env => env.GetEnvironmentVariable("JWT_KEY")).Returns(EnvironmentWrapper.JWT_KEY_TEST);
            userRepository.Setup(repo => repo.SignUp(req.Leader.Email, req.Leader.Password)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync(user);

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

            var company = companyTets;
            company.Type = CompanyTypeFactory.GetCompanyType(COMPANY_TYPE.SMALL);
            company.Sector = CompanySectorFactory.GetCompanySector(COMPANY_SECTOR.PRIMARY);

            var req = new AuthCompanyReq
            {
                Company = company,
                Leader = userTest
            };

            var user = new User
            {
                Id = new Guid(),
                Name = req.Leader.Name,
                LastName = req.Leader.LastName,
                Phone = req.Leader.Phone,
                Cedula = req.Leader.Cedula,
                Email = req.Leader.Email
            };

            environmentMock.Setup(env => env.GetEnvironmentVariable("JWT_KEY")).Returns(EnvironmentWrapper.JWT_KEY_TEST);
            userRepository.Setup(repo => repo.SignUp(req.Leader.Email, req.Leader.Password)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync(user);
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
    }
}
