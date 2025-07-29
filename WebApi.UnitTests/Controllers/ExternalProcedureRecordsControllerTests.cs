using Application.Entity.ExternalProcedureRecords.Commands.ChangeType;
using Application.Entity.ExternalProcedureRecords.Commands.Create;
using Application.Entity.ExternalProcedureRecords.Commands.SoftDelete;
using Application.Entity.ExternalProcedureRecords.Queries;
using Application.Entity.ExternalProcedureRecords.Queries.Get;
using Application.Entity.ExternalProcedureRecords.Queries.GetAll;
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
    public class ExternalProcedureRecordsControllerTests : ExternalProcedureRecordsControllerTestsTheoryData
    {
        private readonly ISender _sender;
        private readonly ExternalProcedureRecordsController _controller;
        private readonly Guid _patientCardId;
        private readonly Guid _typeId;
        private readonly string _typeName;
        private readonly Guid _id;
        private readonly ExternalProcedureRecordFilter _filter;
        private readonly ExternalProcedureRecordResponse _response;
        public ExternalProcedureRecordsControllerTests()
        {
            _patientCardId = Guid.NewGuid();
            _typeId = Guid.NewGuid();
            _id = Guid.NewGuid();
            _response = new(_id, _typeId, "");
            _typeName = "type";
            _filter = new() { Typename = _typeName, };
            _sender = Substitute.For<ISender>();

            _controller = new ExternalProcedureRecordsController(_sender)
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
            var command = new ExternalProcedureRecordCreateCommand(_patientCardId, _typeId, "");

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
            var command = new ExternalProcedureRecordCreateCommand(_patientCardId, _typeId, "");
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
            var command = new ExternalProcedureRecordChangeTypeCommand(_id, _typeId);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.ChangeType(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id);

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenChangeTypeCommandFails()
        {
            // Arrange
            var command = new ExternalProcedureRecordChangeTypeCommand(_id, _typeId);
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
            var command = new ExternalProcedureRecordSoftDeleteCommand(_id);

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
            var command = new ExternalProcedureRecordSoftDeleteCommand(_id);
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
            var list = new List<ExternalProcedureRecordResponse>() { _response };
            _sender.Send(Arg.Any<ExternalProcedureRecordGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

            // Act
            var result = await _controller.GetAll(_filter, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(list);

            await _sender.Received(1).Send(Arg.Any<ExternalProcedureRecordGetAllQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenGetAllQueryFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<ExternalProcedureRecordGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<ExternalProcedureRecordResponse>>(error));

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
            var list = new List<ExternalProcedureRecordResponse>() { _response };
            _sender.Send(Arg.Any<ExternalProcedureRecordGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

            // Act
            var result = await _controller.Take(_filter, 0, 1, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(list);

            await _sender.Received(1).Send(Arg.Any<ExternalProcedureRecordGetAllQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenTakeCommandFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<ExternalProcedureRecordGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<ExternalProcedureRecordResponse>>(error));

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
            _sender.Send(Arg.Any<ExternalProcedureRecordGetQuery>(), Arg.Any<CancellationToken>()).Returns(_response);

            // Act
            var result = await _controller.Get(_id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_response);

            await _sender.Received(1).Send(Arg.Any<ExternalProcedureRecordGetQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenGetCommandFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<ExternalProcedureRecordGetQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<ExternalProcedureRecordResponse>(error));

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
            var method = typeof(ExternalProcedureRecordsController).GetMethod(methodName);

            // Assert
            method.Should().BeDecoratedWith<AuthorizeAttribute>(attr =>
                attr.Policy == AuthorizePolicy.UserOnly);
        }
    }
}