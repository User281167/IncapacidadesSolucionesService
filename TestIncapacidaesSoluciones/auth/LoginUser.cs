
using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using IncapacidadesSoluciones.Dto.auth;
using Moq;
using Microsoft.AspNetCore.Mvc;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Utilities.Role;

namespace TestIncapacidadesSoluciones.auth
{
    public class LoginUser
    {
        Mock<IAccessCodeRepository> accessCodeRepository = new Mock<IAccessCodeRepository>();
        Mock<ICompanyRepository> companyRepository;
        Mock<IUserRepository> userRepository = new Mock<IUserRepository>();
        Mock<IEnvironmentWrapper> environmentMock;
        AuthService authService;
        AuthController authController;

        public LoginUser()
        {
            companyRepository = new Mock<ICompanyRepository>();
            authService = new AuthService(userRepository.Object, companyRepository.Object, accessCodeRepository.Object);
            authController = new AuthController(authService);
            environmentMock = new Mock<IEnvironmentWrapper>();
        }

        [Fact]
        public async void LoginUser_BadEmail()
        {
            var res = await authController.Login(new AuthLoginReq() { Email = "test", Password = "test" });
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void LoginUser_BadCredencials()
        {
            var res = await authController.Login(new AuthLoginReq() { Email = "test@mail.com", Password = "test" });
            
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("Error al iniciar sesión comprueba tus credenciales.", bad.Value);
        }

        [Fact]
        public async void LoginUser_Ok()
        {
            User user = new User()
            {
                Name = "test",
                Role = UserRoleFactory.GetRoleName(USER_ROLE.COLLABORATOR)
            };
            

            Environment.SetEnvironmentVariable("JWT_KEY", EnvironmentWrapper.JWT_KEY_TEST);
            environmentMock.Setup(env => env.GetEnvironmentVariable("JWT_KEY")).Returns(EnvironmentWrapper.JWT_KEY_TEST);

            userRepository.
                Setup(repo => repo.SignIn("test@mail.com", "test")).
                ReturnsAsync(user);

            var res = await authController.Login(new AuthLoginReq() { Email = "test@mail.com", Password = "test" });
            var ok = Assert.IsType<OkObjectResult>(res);
            var authRes = ok.Value as AuthRes;

            Assert.NotEmpty(authRes.Token);
            Assert.Null(authRes.ErrorMessage);
        }
    }
}
