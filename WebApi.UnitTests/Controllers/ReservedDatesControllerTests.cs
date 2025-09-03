using Application.Entity.ReservedDates.Commands.Create;
using Application.Entity.ReservedDates.Commands.SoftDelete;
using Application.Entity.ReservedDates.Queries;
using Application.Entity.ReservedDates.Queries.Get;
using Application.Entity.ReservedDates.Queries.GetAll;
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
	public class ReservedDatesControllerTests : ReservedDatesControllerTestsTheoryData
	{
		private readonly ISender _sender;
		private readonly ReservedDatesController _controller;
		private readonly Guid _id;
		private readonly ReservedDateFilter _filter;
		private readonly ReservedDateResponse _response;
		public ReservedDatesControllerTests()
		{
			_id = Guid.NewGuid();
			_response = new(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Domain.Enums.ReservedDateType.None);
			_filter = new() { Type = "None" };
			_sender = Substitute.For<ISender>();

			_controller = new ReservedDatesController(_sender)
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
			var command = new ReservedDateCreateCommand(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Domain.Enums.ReservedDateType.None);

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
			var command = new ReservedDateCreateCommand(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Domain.Enums.ReservedDateType.None);
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
		public async Task Should_ReturnOkResult_WhenRemoveByIdCommandSucceeds()
		{
			// Arrange
			var command = new ReservedDateSoftDeleteCommand(_id);

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
			var command = new ReservedDateSoftDeleteCommand(_id);
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
			var list = new List<ReservedDateResponse>() { _response };
			_sender.Send(Arg.Any<ReservedDateGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

			// Act
			var result = await _controller.GetAll(_filter, CancellationToken.None);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				.Which.Value.Should().Be(list);

			await _sender.Received(1).Send(Arg.Any<ReservedDateGetAllQuery>(), Arg.Any<CancellationToken>());
		}
		[Fact]
		public async Task Should_ReturnBadRequest_WhenGetAllQueryFails()
		{
			// Arrange
			var error = new Error("Code", "Message");
			_sender.Send(Arg.Any<ReservedDateGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<ReservedDateResponse>>(error));

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
			var list = new List<ReservedDateResponse>() { _response };
			_sender.Send(Arg.Any<ReservedDateGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

			// Act
			var result = await _controller.Take(_filter, 0, 1, CancellationToken.None);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				.Which.Value.Should().Be(list);

			await _sender.Received(1).Send(Arg.Any<ReservedDateGetAllQuery>(), Arg.Any<CancellationToken>());
		}
		[Fact]
		public async Task Should_ReturnBadRequest_WhenTakeCommandFails()
		{
			// Arrange
			var error = new Error("Code", "Message");
			_sender.Send(Arg.Any<ReservedDateGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<ReservedDateResponse>>(error));

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
			_sender.Send(Arg.Any<ReservedDateGetQuery>(), Arg.Any<CancellationToken>()).Returns(_response);

			// Act
			var result = await _controller.Get(_id, CancellationToken.None);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				.Which.Value.Should().Be(_response);

			await _sender.Received(1).Send(Arg.Any<ReservedDateGetQuery>(), Arg.Any<CancellationToken>());
		}
		[Fact]
		public async Task Should_ReturnBadRequest_WhenGetCommandFails()
		{
			// Arrange
			var error = new Error("Code", "Message");
			_sender.Send(Arg.Any<ReservedDateGetQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<ReservedDateResponse>(error));

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
			var method = typeof(ReservedDatesController).GetMethod(methodName);

			// Assert
			method.Should().BeDecoratedWith<AuthorizeAttribute>(attr =>
				attr.Policy == AuthorizePolicy.UserOnly);
		}
	}
}