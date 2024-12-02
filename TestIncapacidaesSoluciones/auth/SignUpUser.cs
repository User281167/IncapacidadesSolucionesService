using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestIncapacidadesSoluciones.auth
{
    public class SignUpUser
    {
        Mock<IAccessCodeRepository> accessCodeRepository = new Mock<IAccessCodeRepository>();
        Mock<ICompanyRepository> companyRepository;
        Mock<IUserRepository> userRepository = new Mock<IUserRepository>();
        Mock<IEnvironmentWrapper> environmentMock;
        AuthService authService;
        AuthController authController;

        AuthUserReq userReq = new AuthUserReq
        {

            AccessCode = "string-3u8Ns1",
            Name = "Jose",
            LastName = "Bedolla",
            Cedula = "1003923",
            Phone = "string",
            Email = "user1@example.com",
            Password = "string"

        };

        public SignUpUser()
        {
            companyRepository = new Mock<ICompanyRepository>();
            authService = new AuthService(userRepository.Object, companyRepository.Object, accessCodeRepository.Object);
            authController = new AuthController(authService);
            environmentMock = new Mock<IEnvironmentWrapper>();
        }

        [Fact]
        public async void SignUpUser_Req_Null()
        {
            var res = await authController.RegisterUser(null);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal("La información no puede ser nula.", bad.Value);
        }

        [Fact]
        public async void SignUpUser_Name_BadRequest()
        {
            var req = userReq;
            req.Name = null;

            var res = await authController.RegisterUser(req);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void SignUpUser_Email_BadRequest()
        {
            var req = userReq;
            req.Email = "test.com";

            var res = await authController.RegisterUser(req);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void SignUpUser_AccessCode_Null_BadRequest()
        {
            var req = userReq;
            req.AccessCode = null;
            var res = await authController.RegisterUser(req);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void SignUpUser_AccessCode_BadRequest()
        {
            var res = await authController.RegisterUser(userReq);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void SignUpUser_UserExists_BadRequest()
        {
            accessCodeRepository.Setup(repo => repo.GetByCode(userReq.AccessCode)).ReturnsAsync(new AccessCode());
            userRepository.Setup(repo => repo.UserExists(userReq.Email, userReq.Cedula)).ReturnsAsync(true);

            var res = await authController.RegisterUser(userReq);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void SignUpUser_Ok()
        {
            var user = new User { Name = userReq.Name };

            Environment.SetEnvironmentVariable("JWT_KEY", EnvironmentWrapper.JWT_KEY_TEST);

            environmentMock.Setup(env => env.GetEnvironmentVariable("JWT_KEY")).Returns(EnvironmentWrapper.JWT_KEY_TEST);
            accessCodeRepository.Setup(repo => repo.GetByCode(userReq.AccessCode)).ReturnsAsync(new AccessCode { ExpirationDate = null });

            userRepository.Setup(repo => repo.UserExists(userReq.Email, userReq.Cedula)).ReturnsAsync(false);
            userRepository.Setup(repo => repo.SignUp(userReq.Email, userReq.Password)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.UpdateByEmail(It.IsAny<User>())).ReturnsAsync(user);

            var res = await authController.RegisterUser(userReq);
            var ok = Assert.IsType<OkObjectResult>(res);
            var authRes = ok.Value as AuthRes;

            Assert.Empty(authRes.ErrorMessage);
            Assert.NotEmpty(authRes.Token);
        }
    }
}
