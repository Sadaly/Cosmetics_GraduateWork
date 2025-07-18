using Application.Entity.Users.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Users.Commands
{
    public class UserCreateCommandTests : TestsTheoryData
    {
        private readonly UserCreateCommandHandler _handler;
        private readonly IUserRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Username _username;
        private readonly Email _email;
        private readonly PasswordHashed _password;

        public UserCreateCommandTests()
        {
            _repository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _username = Username.Create("username").Value;
            _email = Email.Create("str@ing").Value;
            _password = PasswordHashed.Create("password").Value;
            _handler = new UserCreateCommandHandler(_repository, _unitOfWork);

            _repository.AddAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<User>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<User>>());
        }

        [Theory]
        [MemberData(nameof(InvalidNameCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new UserCreateCommand(Name, _email.Value, _password.Value), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidNameCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new UserCreateCommand(Name, _email.Value, _password.Value), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidEmailCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidEmailInput(string Email, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new UserCreateCommand(_username.Value, Email, _password.Value), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidEmailCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidEmailInput(string Email)
        {
            //Act
            var result = await _handler.Handle(new UserCreateCommand(_username.Value, Email, _password.Value), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidPasswordCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidPasswordInput(string Password, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new UserCreateCommand(_username.Value, _email.Value, Password), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidPasswordCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenPasswordInput(string Password)
        {
            //Act
            var result = await _handler.Handle(new UserCreateCommand(_username.Value, _email.Value, Password), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }
    }
}