using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Dto;

namespace TestIncapacidadesSoluciones
{
    public class InabilityTest
    {
        Mock<IUserRepository> userRepository;
        Mock<IInabilityRepository> inabilityRepository;
        InabilityService inabilityService;
        InabilityController inabilityController;

        public InabilityTest()
        {
            userRepository = new Mock<IUserRepository>();
            inabilityRepository = new Mock<IInabilityRepository>();
            inabilityService = new InabilityService(inabilityRepository.Object, userRepository.Object);
            inabilityController = new InabilityController(inabilityService);
        }

        [Fact]
        public async void Inability_Req_Null() {
            var res = await inabilityController.AddInability(null);
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Inability_InvalidUser()
        {
            userRepository.Setup(repo => repo.UserExists(It.IsAny<Guid>())).ReturnsAsync(false);

            var res = await inabilityController.AddInability(new InabilityReq());
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Inability_Ok()
        {
            userRepository.Setup(repo => repo.UserExists(It.IsAny<Guid>())).ReturnsAsync(true);
            inabilityRepository.Setup(repo => repo.Insert(It.IsAny<Inability>())).ReturnsAsync(new Inability());

            var res = await inabilityController.AddInability(new InabilityReq());
            Assert.IsType<OkObjectResult>(res);
        }
    }
}
