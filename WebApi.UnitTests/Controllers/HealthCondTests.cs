using Application.Entity.HealthConds.Commands.ChangeType;
using Application.Entity.HealthConds.Commands.Create;
using Application.Entity.HealthConds.Commands.SoftDelete;
using Application.Entity.HealthConds.Queries;
using Application.Entity.HealthConds.Queries.Get;
using Application.Entity.HealthConds.Queries.GetAll;
using Domain.Shared;
using Domain.SupportData.Filters;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Policies;
using WebApi.UnitTests.TheoryData;

namespace WebApi.UnitTests.Controllers
{
    public class HealthCondsControllerTests : HealthCondsControllerTestsTheoryData
    {
        private readonly ISender _sender;
        private readonly HealthCondsController _controller;
        private readonly Guid _patientCardId;
        private readonly Guid _typeId;
        private readonly string _typeName;
        private readonly Guid _id;
        private readonly HealthCondFilter _filter;
        private readonly HealthCondResponse _response;
        public HealthCondsControllerTests()
        {
            _patientCardId = Guid.NewGuid();
            _typeId = Guid.NewGuid();
            _id = Guid.NewGuid();
            _response = new(_id, _typeId);
            _typeName = "type";
            _filter = new() { Typename = _typeName, };
            _sender = Substitute.For<ISender>();

            _controller = new HealthCondsController(_sender)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task Should_ReturnOkResult_WhenCreateCommandSucceeds()
        {
            // Arrange
            var command = new HealthCondCreateCommand(_patientCardId, _typeId);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.Create(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id);

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenCreateCommandFails()
        {
            // Arrange
            var command = new HealthCondCreateCommand(_patientCardId, _typeId);
            var error = new Error("Code", "Message");

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(Result.Failure<Guid>(error));

            // Act
            var result = await _controller.Create(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }
        [Fact]
        public async Task Should_ReturnOkResult_WhenChangeTypeCommandSucceeds()
        {
            // Arrange
            var command = new HealthCondChangeTypeCommand(_id, _typeId);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.ChangeType(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id);

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenChangeTypeCommandFails()
        {
            // Arrange
            var command = new HealthCondChangeTypeCommand(_id, _typeId);
            var error = new Error("Code", "Message");

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(Result.Failure<Guid>(error));

            // Act
            var result = await _controller.ChangeType(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }
        [Fact]
        public async Task Should_ReturnOkResult_WhenRemoveByIdCommandSucceeds()
        {
            // Arrange
            var command = new HealthCondSoftDeleteCommand(_id);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.RemoveById(command.Id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id);

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenRemoveByIdCommandFails()
        {
            // Arrange
            var command = new HealthCondSoftDeleteCommand(_id);
            var error = new Error("Code", "Message");

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(Result.Failure<Guid>(error));

            // Act
            var result = await _controller.RemoveById(command.Id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }
        [Fact]
        public async Task Should_ReturnOkResult_WhenGetAllQuerySucceeds()
        {
            // Arrange
            var list = new List<HealthCondResponse>() { _response };
            _sender.Send(Arg.Any<HealthCondGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

            // Act
            var result = await _controller.GetAll(_filter, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(list);

            await _sender.Received(1).Send(Arg.Any<HealthCondGetAllQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenGetAllQueryFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<HealthCondGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<HealthCondResponse>>(error));

            // Act
            var result = await _controller.GetAll(_filter, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }
        [Fact]
        public async Task Should_ReturnOkResult_WhenTakeCommandSucceeds()
        {
            // Arrange
            var list = new List<HealthCondResponse>() { _response };
            _sender.Send(Arg.Any<HealthCondGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

            // Act
            var result = await _controller.Take(_filter, 0, 1, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(list);

            await _sender.Received(1).Send(Arg.Any<HealthCondGetAllQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTakeCommandFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<HealthCondGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<HealthCondResponse>>(error));

            // Act
            var result = await _controller.Take(_filter, 0, 1, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }
        [Fact]
        public async Task Should_ReturnOkResult_WhenGetCommandSucceeds()
        {
            // Arrange
            _sender.Send(Arg.Any<HealthCondGetQuery>(), Arg.Any<CancellationToken>()).Returns(_response);

            // Act
            var result = await _controller.Get(_id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_response);

            await _sender.Received(1).Send(Arg.Any<HealthCondGetQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenGetCommandFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<HealthCondGetQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<HealthCondResponse>(error));

            // Act
            var result = await _controller.Get(_id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }

        [Theory]
        [MemberData(nameof(UserOnlyPolicyMethodsName))]
        public void Should_HaveUserOnlyAuthorization_WhenCommandCalled(string methodName)
        {
            // Arrange
            var method = typeof(HealthCondsController).GetMethod(methodName);

            // Assert
            method.Should().BeDecoratedWith<AuthorizeAttribute>(attr =>
                attr.Policy == AuthorizePolicy.UserOnly);
        }
    }
}