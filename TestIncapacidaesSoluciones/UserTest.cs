using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using IncapacidadesSoluciones.Utilities.Role;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestIncapacidadesSoluciones
{
    public class UserTest
    {
        Mock<IUserRepository> userRepository = new Mock<IUserRepository>();
        Mock<IInabilityRepository> inabilityRepository = new Mock<IInabilityRepository>();
        UserService userService;
        UserController userController;

        UserReq user = new UserReq()
        {
            Id = Guid.NewGuid(),
            Name = "test",
            LastName = "test",
            Phone = "test",
            Cedula = "test",
        };

        public UserTest()
        {
            userService = new UserService(userRepository.Object, inabilityRepository.Object);
            userController = new UserController(userService);
        }

        [Fact]
        public async void UpdateUser_IdNotFound()
        {
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            var res = await userController.UpdateUserInfo(user);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void UpdateUser_Ok()
        {
            User userReturn = new User();
            userReturn.Id = user.Id;
            userReturn.Name = user.Name;
            userReturn.LastName = user.LastName;
            userReturn.Phone = user.Phone;
            userReturn.Cedula = user.Cedula;

            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            userRepository.Setup(repo => repo.UpdateByEmail(It.IsAny<User>())).ReturnsAsync(userReturn);

            var res = await userController.UpdateUserInfo(user);
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<User>;

            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
            Assert.Equal(apiRes.Data.Name, user.Name);
        }

        [Fact]
        public async void GetUserInfo_Ok()
        {
            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123",
                Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER)
            };


            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);

            var res = await userController.GetUserInfo(user.Id, leader.Id);
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<User>;

            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
            Assert.Equal(apiRes.Data.Name, user.Name);
        }

        [Fact]
        public async void SearchUser_ErrorRole()
        {
            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123",
                Role = UserRoleFactory.GetRoleName(USER_ROLE.COLLABORATOR)
            };

            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);

            var res = await userController.SearchUser(leader.Id, null, null, null);
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = (res as BadRequestObjectResult)?.Value as ApiRes<List<User>>;

            Assert.NotNull(apiRes);
            Assert.Equal("No tienes permisos para realizar esta operación.", apiRes.Message);
        }

        [Fact]
        public async void SearchUser_Ok()
        {
            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123",
                Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER)
            };

            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.GetByNameOrCedula(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).ReturnsAsync(new List<User>());

            var res = await userController.SearchUser(leader.Id, null, null, null);
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async void GetSpecialRoles_Ok()
        {
            userRepository.Setup(
                repo => repo.GetById(It.IsAny<Guid>())
            ).ReturnsAsync(new User()
            {
                Role = UserRoleFactory.GetRoleName(USER_ROLE.LEADER)
            });

            userRepository.Setup(repo => repo.GetSpecialRoles(It.IsAny<string>())).ReturnsAsync(new List<User>());

            var res = await userController.GetSpecialRoles(new Guid());
            var ok = Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async Task GetNotifications()
        {
            userRepository.Setup(
                repo => repo.GetNotifications(It.IsAny<Guid>())
            ).ReturnsAsync(new List<Notification>());

            var res = await userController.GetNotifications(new Guid());
            var ok = Assert.IsType<OkObjectResult>(res);
            var resOk = ok.Value as ApiRes<List<Notification>>;
            Assert.NotNull(resOk);
            Assert.True(resOk.Success);
            Assert.NotNull(resOk.Data);
        }

        [Fact]
        public async void AddNotification_Error()
        {
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new User());
            inabilityRepository.Setup(
                repo => repo.GetById(It.IsAny<Guid>())
            ).ReturnsAsync(new Inability()
            {
                CollaboratorId = new Guid("00000000-0000-0000-0000-000000000003")
            });

            var req = new AddNotificationReq()
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000001"),
                InabilityId = new Guid("00000000-0000-0000-0000-000000000002"),
                Title = "test",
                Message = "test"
            };

            var res = await userController.AddNotification(req);
            Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = (res as BadRequestObjectResult)?.Value as ApiRes<Notification>;
            Assert.NotNull(apiRes);
            Assert.False(apiRes.Success);
        }

        [Fact]
        public async void AddNotification_Ok()
        {
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new User());
            inabilityRepository.Setup(
                repo => repo.GetById(It.IsAny<Guid>())
            ).ReturnsAsync(new Inability()
            {
                CollaboratorId = new Guid("00000000-0000-0000-0000-000000000001")
            });

            userRepository.Setup(
                repo => repo.AddNotification(It.IsAny<Notification>())
            ).ReturnsAsync(new Notification());

            var req = new AddNotificationReq()
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000001"),
                InabilityId = new Guid("00000000-0000-0000-0000-000000000002"),
                Title = "test",
                Message = "test"
            };

            var res = await userController.AddNotification(req);
            Assert.IsType<OkObjectResult>(res);
            var apiRes = (res as OkObjectResult)?.Value as ApiRes<Notification>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
        }
    }
}