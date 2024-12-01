using IncapacidadesSoluciones.Controllers;
using IncapacidadesSoluciones.Repositories;
using IncapacidadesSoluciones.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using IncapacidadesSoluciones.Dto.Inability;
using IncapacidadesSoluciones.Models;
using IncapacidadesSoluciones.Dto;
using IncapacidadesSoluciones.Utilities;

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
        public async void Inability_Req_Null()
        {
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

        [Fact]
        public async void Inability_GetList_Error()
        {
            userRepository.Setup(repo => repo.UserExists(It.IsAny<Guid>())).ReturnsAsync(true);

            var res = await inabilityController.GetAllByUser(new Guid());
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Inability_GetList_Ok()
        {
            userRepository.Setup(repo => repo.UserExists(It.IsAny<Guid>())).ReturnsAsync(true);
            inabilityRepository.Setup(
                repo => repo.GetUserInabilities(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Inability>(new Inability[] { new Inability() })
            );

            var res = await inabilityController.GetAllByUser(new Guid());
            var ok = Assert.IsType<OkObjectResult>(res);

            var apiRes = ok.Value as ApiRes<List<Inability>>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
            Assert.NotEmpty(apiRes.Data);
        }

        [Fact]
        public async void Inability_GetNoAccepted_Error()
        {
            var res = await inabilityController.GetNoAccepted(new Guid());
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Inability_GetNoAccepted_Ok()
        {
            userRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new User());
            inabilityRepository.Setup(
                repo => repo.GetNoAccepted(It.IsAny<string>()))
                .ReturnsAsync(new List<Inability>(new Inability[] { new Inability() })
            );

            var res = await inabilityController.GetNoAccepted(new Guid());
            var ok = Assert.IsType<OkObjectResult>(res);

            var apiRes = ok.Value as ApiRes<List<Inability>>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
            Assert.NotEmpty(apiRes.Data);
        }

        [Fact]
        public async void Inability_UpdateState_Error()
        {
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new Inability());

            var res = await inabilityController.AcceptInability(new Guid());
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Inability_UpdateState_Ok()
        {
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new Inability());
            userRepository.Setup(repo => repo.GetCollaboratorById(It.IsAny<Guid>())).ReturnsAsync(new Collaborator());
            userRepository.Setup(repo => repo.UpdateCollaborator(It.IsAny<Collaborator>())).ReturnsAsync(new Collaborator());

            inabilityRepository.Setup(
                repo => repo.Update(It.IsAny<Inability>())
            ).ReturnsAsync(new Inability() { State = "ACCEPTED" });

            var res = await inabilityController.AcceptInability(new Guid());
            var ok = Assert.IsType<OkObjectResult>(res);

            var apiRes = ok.Value as ApiRes<Inability>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
            Assert.Equal("ACCEPTED", apiRes.Data.State);
        }

        [Fact]
        public async void Inability_Advise_Error()
        {
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new Inability()
            {
                State = InabilityStateFactory.GetState(INABILITY_STATE.PENDING)
            });

            // Inability state need to be accepted
            var res = await inabilityController.AdviseInability(new Guid(), true);
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Inability_Advise_Ok()
        {
            var inability = new Inability()
            {
                State = InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED)
            };

            // Inability state need to be accepted
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(inability);
            inabilityRepository.Setup(
                repo => repo.Update(It.IsAny<Inability>())
            ).ReturnsAsync(new Inability() { Advise = true });

            var res = await inabilityController.AdviseInability(new Guid(), true);
            var ok = Assert.IsType<OkObjectResult>(res);

            var apiRes = ok.Value as ApiRes<Inability>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
            Assert.True(apiRes.Data.Advise);
        }
    }
}
