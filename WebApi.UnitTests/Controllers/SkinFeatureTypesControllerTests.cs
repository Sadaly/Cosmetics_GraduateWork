using Application.Entity.SkinFeatureTypes.Commands.Create;
using Application.Entity.SkinFeatureTypes.Commands.SoftDelete;
using Application.Entity.SkinFeatureTypes.Commands.Update;
using Application.Entity.SkinFeatureTypes.Queries;
using Application.Entity.SkinFeatureTypes.Queries.Get;
using Application.Entity.SkinFeatureTypes.Queries.GetAll;
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
    public class SkinFeatureTypesControllerTests : SkinFeatureTypesControllerTestsTheoryData
    {
        private readonly ISender _sender;
        private readonly SkinFeatureTypesController _controller;
        private readonly string _typeName;
        private readonly Guid _id;
        private readonly SkinFeatureTypeFilter _filter;
        private readonly SkinFeatureTypeResponse _response;
        public SkinFeatureTypesControllerTests()
        {
            _id = Guid.NewGuid();
            _typeName = "type";
            _response = new(_id, _typeName);
            _filter = new() { Typename = _typeName, };
            _sender = Substitute.For<ISender>();

            _controller = new SkinFeatureTypesController(_sender)
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
            var command = new SkinFeatureTypeCreateCommand(_typeName);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.Create(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id);

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenCreateCommandFails()
        {
            // Arrange
            var command = new SkinFeatureTypeCreateCommand(_typeName);
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
        public async Task Should_ReturnOkResult_WhenUpdateCommandSucceeds()
        {
            // Arrange
            var command = new SkinFeatureTypeUpdateCommand(_id, _typeName);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.Update(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id);

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenUpdateCommandFails()
        {
            // Arrange
            var command = new SkinFeatureTypeUpdateCommand(_id, _typeName);
            var error = new Error("Code", "Message");

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(Result.Failure<Guid>(error));

            // Act
            var result = await _controller.Update(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }
        [Fact]
        public async Task Should_ReturnOkResult_WhenRemoveByIdCommandSucceeds()
        {
            // Arrange
            var command = new SkinFeatureTypeSoftDeleteCommand(_id);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.RemoveById(command.Id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id);

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenRemoveByIdCommandFails()
        {
            // Arrange
            var command = new SkinFeatureTypeSoftDeleteCommand(_id);
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
            var list = new List<SkinFeatureTypeResponse>() { _response };
            _sender.Send(Arg.Any<SkinFeatureTypeGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

            // Act
            var result = await _controller.GetAll(_filter, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(list);

            await _sender.Received(1).Send(Arg.Any<SkinFeatureTypeGetAllQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenGetAllQueryFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<SkinFeatureTypeGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<SkinFeatureTypeResponse>>(error));

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
            var list = new List<SkinFeatureTypeResponse>() { _response };
            _sender.Send(Arg.Any<SkinFeatureTypeGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

            // Act
            var result = await _controller.Take(_filter, 0, 1, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(list);

            await _sender.Received(1).Send(Arg.Any<SkinFeatureTypeGetAllQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenTakeCommandFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<SkinFeatureTypeGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<SkinFeatureTypeResponse>>(error));

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
            _sender.Send(Arg.Any<SkinFeatureTypeGetQuery>(), Arg.Any<CancellationToken>()).Returns(_response);

            // Act
            var result = await _controller.Get(_id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_response);

            await _sender.Received(1).Send(Arg.Any<SkinFeatureTypeGetQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenGetCommandFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<SkinFeatureTypeGetQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<SkinFeatureTypeResponse>(error));

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
            var method = typeof(SkinFeatureTypesController).GetMethod(methodName);

            // Assert
            method.Should().BeDecoratedWith<AuthorizeAttribute>(attr =>
                attr.Policy == AuthorizePolicy.UserOnly);
        }
    }
}