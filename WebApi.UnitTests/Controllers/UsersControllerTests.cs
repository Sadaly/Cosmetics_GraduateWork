using Application.Entity.Users.Commands.Create;
using Application.Entity.Users.Commands.Login;
using Application.Entity.Users.Commands.SoftDelete;
using Application.Entity.Users.Commands.Update;
using Application.Entity.Users.Queries;
using Application.Entity.Users.Queries.Get;
using Application.Entity.Users.Queries.GetAll;
using Domain.Errors;
using Domain.Shared;
using Domain.SupportData.Filters;
using FluentAssertions;
using Infrastructure.IService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using WebApi.Controllers;
using WebApi.DTO.UserDTO;
using WebApi.Policies;
using WebApi.UnitTests.TheoryData;

namespace WebApi.UnitTests.Controllers
{
    public class UsersControllerTests : UsersControllerTestsTheoryData
    {
        private readonly ISender _sender;
        private readonly ITokenService _tokenService;
        private readonly UsersController _controller;
        private readonly string _name;
        private readonly string _email;
        private readonly string _password;
        private readonly string _token;
        private readonly Guid _id;
        private readonly UserFilter _filter;
        private readonly UserResponse _response;
        public UsersControllerTests()
        {            
            _name = "name";
            _email = "str@ing";
            _password = "password";
            _id = Guid.NewGuid();
            _token = _id.ToString();
            _response = new(_id, _name, _email, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
            _filter = new() { Username = _name, };
            _sender = Substitute.For<ISender>();
            _tokenService = Substitute.For<ITokenService>();
            _tokenService.GetClaim(Arg.Any<string>(), Arg.Any<string>()).Returns(_token);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _token),
                new Claim(ClaimTypes.Email, _email),
                new Claim(ClaimTypes.Role, "user"),
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller = new UsersController(_sender, _tokenService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrincipal
                    }
                }
            };
        }

        [Fact]
        public async Task Should_ReturnOkResult_WhenCreateCommandSucceeds()
        {
            // Arrange
            var command = new UserCreateCommand(_name, _email, _password);

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
            var command = new UserCreateCommand(_name, _email, _password);
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
            var command = new UserUpdateCommand(_id, _name, _email, _password);
            var dto = new UserUpdateRequest(_name, _email, _password);
            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_token);
            // Act
            var result = await _controller.UpdateSelf(dto, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id.ToString());

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenUpdateCommandFails()
        {
            // Arrange
            var command = new UserUpdateCommand(_id, _name, _email, _password);
            var dto = new UserUpdateRequest(_name, _email, _password);
            var error = new Error("Code", "Message");
            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(Result.Failure<string>(error));

            // Act
            var result = await _controller.UpdateSelf(dto, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenUpdateCommandIdEmpty()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new();
            var command = new UserUpdateCommand(_id, _name, _email, _password);
            var dto = new UserUpdateRequest(_name, _email, _password);

            // Act
            var result = await _controller.UpdateSelf(dto, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(WebErrors.UserController.UpdateSelf.EmptyId.Code);
        }
        [Fact]
        public async Task Should_ReturnOkResult_WhenRemoveByIdCommandSucceeds()
        {
            // Arrange
            var anotherId = Guid.NewGuid();
            var command = new UserSoftDeleteCommand(anotherId);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(anotherId);

            // Act
            var result = await _controller.RemoveById(command.Id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(anotherId);

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenRemoveByIdCommandIdSameAsUsers()
        {
            // Arrange
            var command = new UserSoftDeleteCommand(_id);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.RemoveById(command.Id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(WebErrors.UserController.RemoveById.SelfDeleteFobbiden.Code);
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenRemoveByIdCommandIdEmpty()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new();
            var command = new UserSoftDeleteCommand(_id);

            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_id);

            // Act
            var result = await _controller.RemoveById(command.Id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(WebErrors.UserController.RemoveById.EmptyId.Code);
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenRemoveByIdCommandFails()
        {
            // Arrange
            var anotherId = Guid.NewGuid();
            var command = new UserSoftDeleteCommand(anotherId);
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
            var list = new List<UserResponse>() { _response };
            _sender.Send(Arg.Any<UserGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

            // Act
            var result = await _controller.GetAll(_filter, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(list);

            await _sender.Received(1).Send(Arg.Any<UserGetAllQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenGetAllQueryFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<UserGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<UserResponse>>(error));

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
            var list = new List<UserResponse>() { _response };
            _sender.Send(Arg.Any<UserGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(list);

            // Act
            var result = await _controller.Take(_filter, 0, 1, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(list);

            await _sender.Received(1).Send(Arg.Any<UserGetAllQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenTakeCommandFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<UserGetAllQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<List<UserResponse>>(error));

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
            _sender.Send(Arg.Any<UserGetQuery>(), Arg.Any<CancellationToken>()).Returns(_response);

            // Act
            var result = await _controller.GetById(_id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_response);

            await _sender.Received(1).Send(Arg.Any<UserGetQuery>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenGetCommandFails()
        {
            // Arrange
            var error = new Error("Code", "Message");
            _sender.Send(Arg.Any<UserGetQuery>(), Arg.Any<CancellationToken>()).Returns(Result.Failure<UserResponse>(error));

            // Act
            var result = await _controller.GetById(_id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Type.Should().Be(error.Code);
        }

        [Fact]
        public void Should_ReturnOkResultWithNulls_WhenGetSelfCommandSucceeds()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _token),
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = claimsPrincipal;

            // Act
            var result = _controller.GetSelf();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<(string, string?, string?)>()
                .Which.Should().Be((_id.ToString(), null, null));
        }
        [Fact]
        public void Should_ReturnOkResult_WhenGetSelfCommandSucceeds()
        {
            // Arrange

            // Act
            var result = _controller.GetSelf();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<(string, string?, string?)>()
                .Which.Should().Be((_id.ToString(), _email, "user"));
        }
        [Fact]
        public void Should_ReturnOkResult_WhenLogoutCommandSucceeds()
        {
            // Arrange

            // Act
            var result = _controller.Logout();

            // Assert
            result.Should().BeOfType<OkResult>()
                .Which.StatusCode.Should().Be(200);
        }
        [Fact]
        public async Task Should_ReturnOkResult_WhenLoginCommandSucceeds()
        {
            // Arrange
            var command = new UserLoginCommand(_email, _password);
            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(_token);
            // Act
            var result = await _controller.Login(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(_id.ToString());

            await _sender.Received(1).Send(command, Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenLoginCommandFails()
        {
            // Arrange
            var command = new UserLoginCommand(_email, _password);
            var error = new Error("Code", "Message");
            _sender.Send(command, Arg.Any<CancellationToken>()).Returns(Result.Failure<string>(error));

            // Act
            var result = await _controller.Login(command, CancellationToken.None);

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
            var method = typeof(UsersController).GetMethod(methodName);

            // Assert
            method.Should().BeDecoratedWith<AuthorizeAttribute>(attr =>
                attr.Policy == AuthorizePolicy.UserOnly);
        }
    }
}