using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace TestIncapacidadesSoluciones.auth
{
    public class AccessCodeTest
    {
        Mock<IAccessCodeRepository> accessCodeRepository = new Mock<IAccessCodeRepository>();
        Mock<ICompanyRepository> companyRepository;
        AuthService authService;
        AuthController authController;

        AuthAccessCodeReq code = new AuthAccessCodeReq { Id = new Guid(), CompanyId = new Guid()};

        public AccessCodeTest() { 
            accessCodeRepository = new Mock<IAccessCodeRepository>();
            companyRepository = new Mock<ICompanyRepository>();
            authService = new AuthService(null, companyRepository.Object, accessCodeRepository.Object);
            authController = new AuthController(authService);
        }

        [Fact]
        public void CreateAccessCode()
        {
            // Act
            var res = IncapacidadesSoluciones.Models.AccessCode.GenerateCode("test", 6);

            // Assert
            Assert.NotNull(res);
            Assert.Equal(6 + 5, res.Length);
            Assert.Equal("test-", res.Substring(0, 5));
        }

        [Fact]
        public async void AddAccessCode_CompanyError()
        {
            // Act
            var res = await authController.AddAccessCode(code);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("No se pudo encontrar la empresa.", bad.Value);
        }

        [Fact]
        public async void AddAccessCode_Exists()
        {
            // Arange
            companyRepository.Setup(repo => repo.GetCompany(code.CompanyId)).ReturnsAsync(new Company());

            // Act
            var res = await authController.AddAccessCode(code);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async void AddAccessCode_Update()
        {
            // Arange
            companyRepository.Setup(repo => repo.GetCompany(code.CompanyId)).ReturnsAsync(new Company());
            accessCodeRepository.Setup(repo => repo.GetById(code.Id)).ReturnsAsync(new AccessCode());
            //accessCodeRepository.Setup(repo => repo.Exists("test")).ReturnsAsync(true);

            // Act
            var res = await authController.AddAccessCode(code);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<AccessCode>;

            Assert.Equal("Código de acceso actualizado con exito.", apiRes.Message);
        }
    }
}
