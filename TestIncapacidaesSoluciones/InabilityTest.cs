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

        [Fact]
        public async void Inability_Payment_NitError()
        {
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new Inability()
            {
                State = InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED)
            });

            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123456789"
            };

            InabilityPaymentReq req = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                LeaderId = leader.Id,
            };

            Inability inability = new()
            {
                CollaboratorId = user.Id
            };

            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);
            inabilityRepository.Setup(repo => repo.GetById(req.Id)).ReturnsAsync(inability);

            var res = await inabilityController.PaymentInability(req);
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Inability_Payment_NoAcceptedError()
        {
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(new Inability()
            {
                State = InabilityStateFactory.GetState(INABILITY_STATE.PENDING)
            });

            // user and leader with same nit
            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123"
            };

            InabilityPaymentReq req = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                LeaderId = leader.Id,
            };

            Inability inability = new()
            {
                CollaboratorId = user.Id
            };

            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);
            inabilityRepository.Setup(repo => repo.GetById(req.Id)).ReturnsAsync(inability);

            var res = await inabilityController.PaymentInability(req);
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Inability_Payment_Ok()
        {
            // get by id, inability add NO_ACCEPTED
            var inability = new Inability()
            {
                State = InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED),
                CollaboratorId = new Guid("00000000-0000-0000-0000-000000000001")
            };

            // user and leader with same nit
            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123456789"
            };

            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123456789"
            };

            InabilityPaymentReq req = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                LeaderId = leader.Id,
            };

            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);
            userRepository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);

            // update
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(inability);
            inabilityRepository.Setup(
                repo => repo.Update(It.IsAny<Inability>())
            ).ReturnsAsync(new Inability()
            {
                CompanyPayment = 100,
                HealthEntityPayment = 200,
                Pay = 300
            });

            var res = await inabilityController.PaymentInability(req);
            var ok = Assert.IsType<OkObjectResult>(res);

            var apiRes = ok.Value as ApiRes<Inability>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
            Assert.Equal(300ul, apiRes.Data.Pay);
        }


        [Fact]
        public async void Inability_AddReplacement_SameIdError()
        {
            // replacement is equals to user
            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123456789"
            };

            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123456789"
            };

            User replacement = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
                CompanyNIT = "123456789"
            };

            Inability inability = new()
            {
                State = InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED),
                CollaboratorId = user.Id
            };

            Collaborator collaborator = new()
            {
                Id = user.Id,
                Inability = true,
                HasReplacement = true
            };

            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(inability);
            userRepository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.GetCollaboratorById(replacement.Id)).ReturnsAsync(collaborator);
            userRepository.Setup(repo => repo.GetCollaboratorById(user.Id)).ReturnsAsync(collaborator);

            var req = new InabilityReplacementReq()
            {
                LeaderId = leader.Id,
                InabilityId = inability.Id,
                ReplacementId = replacement.Id
            };

            var res = await inabilityController.AddReplacementInability(req);
            Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = (res as BadRequestObjectResult)?.Value as ApiRes<Inability>;
            Assert.NotNull(apiRes);
            Assert.Equal("El usuario de reemplazo es el mismo que el usuario de la incapacidad.", apiRes.Message);
        }

        [Fact]
        public async void Inability_AddReplacement_Error()
        {
            // replacement has inability
            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123456789"
            };

            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123456789"
            };

            User replacement = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
                CompanyNIT = "123456789"
            };

            Inability inability = new()
            {
                State = InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED),
                CollaboratorId = user.Id
            };

            Collaborator collaborator = new()
            {
                Id = replacement.Id,
                Inability = true,
                HasReplacement = true
            };

            Collaborator collaboratorReplacement = new()
            {
                Id = replacement.Id,
                Inability = true,
                HasReplacement = true
            };


            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(inability);
            userRepository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.GetCollaboratorById(user.Id)).ReturnsAsync(collaborator);
            userRepository.Setup(repo => repo.GetCollaboratorById(replacement.Id)).ReturnsAsync(collaboratorReplacement);

            var req = new InabilityReplacementReq()
            {
                LeaderId = leader.Id,
                InabilityId = inability.Id,
                ReplacementId = replacement.Id
            };

            var res = await inabilityController.AddReplacementInability(req);
            Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = (res as BadRequestObjectResult)?.Value as ApiRes<Inability>;
            Assert.NotNull(apiRes);
            Assert.Equal("El usuario de reemplazo tiene una incapacidad aceptada.", apiRes.Message);
        }

        [Fact]
        public async void Inability_AddReplacement_Ok()
        {
            // replacement has inability
            User leader = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                CompanyNIT = "123456789"
            };

            User user = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                CompanyNIT = "123456789"
            };

            User replacement = new()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
                CompanyNIT = "123456789"
            };

            Inability inability = new()
            {
                State = InabilityStateFactory.GetState(INABILITY_STATE.ACCEPTED),
                CollaboratorId = user.Id
            };

            Collaborator collaborator = new()
            {
                Id = replacement.Id,
                Inability = true,
                HasReplacement = true
            };

            Collaborator collaboratorReplacement = new()
            {
                Id = replacement.Id,
                Inability = false,
                HasReplacement = false
            };

            userRepository.Setup(repo => repo.GetById(leader.Id)).ReturnsAsync(leader);
            inabilityRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(inability);
            userRepository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);
            userRepository.Setup(repo => repo.GetCollaboratorById(user.Id)).ReturnsAsync(collaborator);
            userRepository.Setup(repo => repo.GetCollaboratorById(replacement.Id)).ReturnsAsync(collaboratorReplacement);

            userRepository.Setup(
                repo => repo.UpdateCollaborator(It.IsAny<Collaborator>())
            ).ReturnsAsync(new Collaborator());

            inabilityRepository.Setup(
                repo => repo.Update(It.IsAny<Inability>())
            ).ReturnsAsync(new Inability() { State = "ACCEPTED" });

            var req = new InabilityReplacementReq()
            {
                LeaderId = leader.Id,
                InabilityId = inability.Id,
                ReplacementId = replacement.Id
            };

            var res = await inabilityController.AddReplacementInability(req);
            var ok = Assert.IsType<OkObjectResult>(res);

            var apiRes = ok.Value as ApiRes<Inability>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
        }

        [Fact]
        public async void Inability_GetPaymentReport_Ok()
        {
            // Given
            inabilityRepository.Setup(
                repo => repo.GetPaymentReport(It.IsAny<string>())
            ).ReturnsAsync(new List<InabilityPaymentRes>());

            userRepository.Setup(
                repo => repo.GetById(It.IsAny<Guid>())
            ).ReturnsAsync(new User());

            // When
            var res = await inabilityController.GetPaymentReport(new Guid());

            // Then
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<List<InabilityPaymentRes>>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
        }

        [Fact]
        public async Task GetNotifications()
        {
            inabilityRepository.Setup(
                repo => repo.GetNotifications(It.IsAny<Guid>())
            ).ReturnsAsync(new List<Notification>());

            var res = await inabilityController.GetNotifications(new Guid());
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<List<Notification>>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
        }

        [Fact]
        public async void AddFile_IdError()
        {
            // user.id and inability.id are different
            // Given
            inabilityRepository.Setup(
                repo => repo.GetById(It.IsAny<Guid>())
            ).ReturnsAsync(new Inability());

            inabilityRepository.Setup(
                repo => repo.AddFile(It.IsAny<AddFileReq>())
            ).ReturnsAsync(new InabilityFile());

            var req = new AddFileReq()
            {
                InabilityId = new Guid("00000000-0000-0000-0000-000000000001"),
                Title = "file.pdf",
                UserId = new Guid("00000000-0000-0000-0000-000000000001")
            };

            // When
            var res = await inabilityController.AddFile(req);

            // Then
            var bad = Assert.IsType<BadRequestObjectResult>(res);
            var apiRes = bad.Value as ApiRes<InabilityFile>;
            Assert.NotNull(apiRes);
            Assert.Equal("El usuario no es el responsable de la incapacidad.", apiRes.Message);
        }

        [Fact]
        public async void AddFile_Ok()
        {
            // user.id and inability.id are different
            // Given
            inabilityRepository.Setup(
                repo => repo.GetById(It.IsAny<Guid>())
            ).ReturnsAsync(new Inability()
            {
                CollaboratorId = new Guid("00000000-0000-0000-0000-000000000001")
            });

            inabilityRepository.Setup(
                repo => repo.AddFile(It.IsAny<AddFileReq>())
            ).ReturnsAsync(new InabilityFile());

            var req = new AddFileReq()
            {
                InabilityId = new Guid("00000000-0000-0000-0000-000000000001"),
                Title = "file.pdf",
                UserId = new Guid("00000000-0000-0000-0000-000000000001")
            };

            // When
            var res = await inabilityController.AddFile(req);

            // Then
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<InabilityFile>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
        }

        [Fact]
        public async void GetFiles()
        {
            // Given
            inabilityRepository.Setup(
                repo => repo.GetFiles(It.IsAny<Guid>())
            ).ReturnsAsync(new List<InabilityFile>());

            // When
            var res = await inabilityController.GetFiles(new Guid());

            // Then
            var ok = Assert.IsType<OkObjectResult>(res);
            var apiRes = ok.Value as ApiRes<List<InabilityFile>>;
            Assert.NotNull(apiRes);
            Assert.True(apiRes.Success);
            Assert.NotNull(apiRes.Data);
        }
    }
}
