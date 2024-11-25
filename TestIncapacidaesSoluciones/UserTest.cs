using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Dto.UserDto;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestIncapacidadesSoluciones
{
    public class UserTest
    {
        Mock<IUserRepository> userRepository = new Mock<IUserRepository>();
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
            userService = new UserService(userRepository.Object);
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
            userRepository.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync(userReturn);

            var res = await userController.UpdateUserInfo(user);
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<User>;

            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.Equal(apiRes.Data.Name, user.Name);
        }
    }
}
