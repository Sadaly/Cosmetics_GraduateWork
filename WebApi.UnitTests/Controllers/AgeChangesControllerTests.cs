using Application.Entity.AgeChanges.Commands.ChangeType;
using Application.Entity.AgeChanges.Commands.Create;
using Application.Entity.AgeChanges.Commands.SoftDelete;
using Application.Entity.AgeChanges.Queries;
using Application.Entity.AgeChanges.Queries.Get;
using Application.Entity.AgeChanges.Queries.GetAll;
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
	public class AgeChangesControllerTests : AgeChangesControllerTestsTheoryData
	{
		private readonly ISender _sender;
		private readonly AgeChangesController _controller;
		private readonly Guid _patientCardId;
		private readonly Guid _typeId;
		private readonly string _typeName;
		private readonly Guid _id;
		private readonly AgeChangeFilter _filter;
		private readonly AgeChangeResponse _response;
		public AgeChangesControllerTests()
		{
			_patientCardId = Guid.NewGuid();
			_typeId = Guid.NewGuid();
			_id = Guid.NewGuid();
			_response = new(_id, _typeId);
			_typeName = "type";
			_filter = new() { Typename = _typeName, };
			_sender = Substitute.For<ISender>();

			_controller = new AgeChangesController(_sender)
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
			var command = new AgeChangeCreateCommand(_patientCardId, _typeId);

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
			var command = new AgeChangeCreateCommand(_patientCardId, _typeId);
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
			var command = new AgeChangeChangeTypeCommand(_id, _typeId);

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
			var command = new AgeChangeChangeTypeCommand(_id, _typeId);
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
			var command = new AgeChangeSoftDeleteCommand(_id);

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
			var command = new AgeChangeSoftDeleteCommand(_id);
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
			var list = new List<AgeChangeResponse>() { _response };
			_sender.Send(Arg.Any<AgeChangeGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

			// Act
			var result = await _controller.GetAll(_filter, CancellationToken.None);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				.Which.Value.Should().Be(list);

			await _sender.Received(1).Send(Arg.Any<AgeChangeGetAllQuery>(), Arg.Any<CancellationToken>());
		}
		[Fact]
		public async Task Should_ReturnBadRequest_WhenGetAllQueryFails()
		{
			// Arrange
			var error = new Error("Code", "Message");
			_sender.Send(Arg.Any<AgeChangeGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<AgeChangeResponse>>(error));

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
			var list = new List<AgeChangeResponse>() { _response };
			_sender.Send(Arg.Any<AgeChangeGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

			// Act
			var result = await _controller.Take(_filter, 0, 1, CancellationToken.None);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				.Which.Value.Should().Be(list);

			await _sender.Received(1).Send(Arg.Any<AgeChangeGetAllQuery>(), Arg.Any<CancellationToken>());
		}
		[Fact]
		public async Task Should_ReturnBadRequest_WhenTakeCommandFails()
		{
			// Arrange
			var error = new Error("Code", "Message");
			_sender.Send(Arg.Any<AgeChangeGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<AgeChangeResponse>>(error));

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
			_sender.Send(Arg.Any<AgeChangeGetQuery>(), Arg.Any<CancellationToken>()).Returns(_response);

			// Act
			var result = await _controller.Get(_id, CancellationToken.None);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				.Which.Value.Should().Be(_response);

			await _sender.Received(1).Send(Arg.Any<AgeChangeGetQuery>(), Arg.Any<CancellationToken>());
		}
		[Fact]
		public async Task Should_ReturnBadRequest_WhenGetCommandFails()
		{
			// Arrange
			var error = new Error("Code", "Message");
			_sender.Send(Arg.Any<AgeChangeGetQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<AgeChangeResponse>(error));

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
			var method = typeof(AgeChangesController).GetMethod(methodName);

			// Assert
			method.Should().BeDecoratedWith<AuthorizeAttribute>(attr =>
				attr.Policy == AuthorizePolicy.UserOnly);
		}
	}
}