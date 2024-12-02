using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.auth;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using IncapacidadesSoluciones.Utilities.Role;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestIncapacidadesSoluciones.auth
{
    public class Role
    {
        private Mock<IUserRepository> userRepository;
        private AuthService authService;
        private AuthController authController;

        private AuthRoleReq authRoleReq = new AuthRoleReq
        {
            LeaderId = new Guid(),
            Name = "Test",
            LastName = "Test",
            Cedula = "12345678",
            Email = "test@test.com",
            Password = "12345678",
            Role = UserRoleFactory.GetRoleName(USER_ROLE.RECEPTIONIST)
        };

        public Role()
        {
            userRepository = new Mock<IUserRepository>();
            authService = new AuthService(userRepository.Object, null, null);
            authController = new AuthController(authService);
        }

        [Fact]
        public async void Role_BadRole()
        {
            var req = authRoleReq;
            req.Role = "bad";

            var res = await authController.CreateRole(req);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = bad.Value as ApiRes<User>;
            Assert.NotNull(apiRes);
            Assert.False(apiRes.Success);
            Assert.Equal("No se puede crear un usuario con credenciales de bad.", apiRes.Message);
        }

        [Fact]
        public async void Role_Leader()
        {
            var req = authRoleReq;
            req.Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER);

            var res = await authController.CreateRole(req);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = bad.Value as ApiRes<User>;
            Assert.NotNull(apiRes);
            Assert.False(apiRes.Success);
            Assert.Equal("No se puede crear un usuario con credenciales de LIDER.", apiRes.Message);
        }

        [Fact]
        public async void Role_Collaborator()
        {
            var req = authRoleReq;
            req.Role = UserRoleFactory.GetRoleName(USER_ROLE.COLLABORATOR);

            var res = await authController.CreateRole(req);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = bad.Value as ApiRes<User>;
            Assert.NotNull(apiRes);
            Assert.False(apiRes.Success);
            Assert.Equal("No se puede crear un usuario con credenciales de COLABORADOR.", apiRes.Message);
        }

        [Fact]
        public async void Role_Bad_UserExists()
        {
            // Arrange
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            userRepository.Setup(repo => repo.UpdateByEmail(It.IsAny<User>())).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.SignUp(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new User());

            var res = await authController.CreateRole(authRoleReq);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = bad.Value as ApiRes<User>;
            Assert.NotNull(apiRes);
            Assert.False(apiRes.Success);
            Assert.Null(apiRes.Data);
            Assert.Equal("Ya existe un usuario con ese correo o cédula registrado.", apiRes.Message);
        }

        [Fact]
        public async void Role_OK()
        {
            // Arrange
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            userRepository.Setup(repo => repo.UpdateByEmail(It.IsAny<User>())).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.SignUp(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new User());

            var res = await authController.CreateRole(authRoleReq);
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<User>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
        }

        [Fact]
        public async void UpdateRole_NitError()
        {
            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "000"
            };

            User check = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
            };

            // Arrange
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(user);
            userRepository.Setup(
                repo => repo.GetByEmailOrCedula(It.IsAny<string>(), It.IsAny<string>())
            ).ReturnsAsync(check);

            var res = await authController.UpdateRole(authRoleReq);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = bad.Value as ApiRes<User>;
            Assert.NotNull(apiRes);
            Assert.False(apiRes.Success);
            Assert.NotEmpty(apiRes.Message);
        }

        [Fact]
        public async void UpdateRole_Ok()
        {
            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "000"
            };

            var req = authRoleReq;
            req.UserId = user.Id;

            // Arrange
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(user);
            userRepository.Setup(
                repo => repo.GetByEmailOrCedula(It.IsAny<string>(), It.IsAny<string>())
            ).ReturnsAsync(user);
            userRepository.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync(user);

            var res = await authController.UpdateRole(authRoleReq);
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<User>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
        }

        [Fact]
        public async void DeleteRole_SameIdError()
        {
            DeleteAuthReq req = new()
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000001"),
                LeaderId = new Guid("00000000-0000-0000-0000-000000000001"),
            };

            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
            };

            // Arrange
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(user);

            var res = await authController.DeleteRole(req);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = bad.Value as ApiRes<bool>;
            Assert.NotNull(apiRes);
            Assert.False(apiRes.Success);
            Assert.NotEmpty(apiRes.Message);
        }

        [Fact]
        public async void DeleteRole_NitError()
        {
            DeleteAuthReq req = new()
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000001"),
                LeaderId = new Guid("00000000-0000-0000-0000-000000000002"),
            };

            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "000"
            };

            // Arrange
            userRepository.Setup(repo => repo.GetById(req.LeaderId)).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(req.UserId)).ReturnsAsync(user);
            
            var res = await authController.DeleteRole(req);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = bad.Value as ApiRes<bool>;
            Assert.NotNull(apiRes);
            Assert.False(apiRes.Success);
            Assert.NotEmpty(apiRes.Message);
        }


        [Fact]
        public async void DeleteRole_Ok()
        {
            DeleteAuthReq req = new()
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000001"),
                LeaderId = new Guid("00000000-0000-0000-0000-000000000002"),
            };

            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "000"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "000"
            };

            // Arrange
            userRepository.Setup(repo => repo.GetById(req.LeaderId)).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(req.UserId)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.Delete(It.IsAny<Guid>()));

            var res = await authController.DeleteRole(req);
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<bool>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.True(apiRes.Data);
        }
    }
}
