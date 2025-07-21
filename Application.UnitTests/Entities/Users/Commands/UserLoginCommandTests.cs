using Application.Abstractions;
using Application.Entity.Users.Commands.Login;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Users.Commands
{
    public class UserLoginCommandTests : TestsTheoryData
    {
        private readonly UserLoginCommandHandler _handler;
        private readonly IUserRepository _repository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly User _user;
        private readonly Username _username;
        private readonly Email _email;
        private readonly PasswordHashed _password;

        public UserLoginCommandTests()
        {
            _repository = Substitute.For<IUserRepository>();
            _jwtProvider = Substitute.For<IJwtProvider>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _username = Username.Create("username").Value;
            _email = Email.Create("str@ing").Value;
            _password = PasswordHashed.Create("password").Value;
            _user = User.Create(_email, _username, _password).Value;

            _handler = new UserLoginCommandHandler(_repository, _jwtProvider);

            _jwtProvider.Generate(Arg.Any<User>())
                .Returns("jwt");

            _repository.GetByEmailAsync(Arg.Is<Result<Email>>(x => x.Value.Value == _user.Email.Value), Arg.Any<CancellationToken>())
                .Returns(_user);

            _repository.GetByEmailAsync(Arg.Is<Result<Email>>(x => x.Value.Value != _user.Email.Value), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<User>(PersistenceErrors.Entity<User>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<User>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnError_WhenInvalidEmailInput()
        {
            //Act
            var result = await _handler.Handle(new UserLoginCommand(_email.Value + "a", "password"), default);

            //Assert
            result.Error.Code.Should().Be(PersistenceErrors.User.IncorrectUsernameOrPassword);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidEmailAndPasswordInput()
        {
            //Act
            var result = await _handler.Handle(new UserLoginCommand(_email.Value, "password"), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Fact]
        public async Task Handle_Should_ReturnError_WhenInvalidPasswordInput()
        {
            //Act
            var result = await _handler.Handle(new UserLoginCommand(_email.Value, PasswordHashed.Create("passwordo").Value.Value), default);

            //Assert
            result.Error.Code.Should().Be(PersistenceErrors.User.IncorrectUsernameOrPassword);
        }
        [Fact]
        public async Task Handle_Should_ReturnError_WhenInvalidEmailAndPasswordInput()
        {
            //Act
            var result = await _handler.Handle(new UserLoginCommand(_email.Value + "a", PasswordHashed.Create("passwordo").Value.Value), default);

            //Assert
            result.Error.Code.Should().Be(PersistenceErrors.User.IncorrectUsernameOrPassword);
        }
    }
}