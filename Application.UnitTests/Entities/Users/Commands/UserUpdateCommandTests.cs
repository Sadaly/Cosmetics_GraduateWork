using Application.Abstractions;
using Application.Entity.Users.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using FluentValidation.TestHelper;
using NSubstitute;

namespace Application.UnitTests.Entities.Users.Commands
{
    public class UserUpdateCommandTests : TestsTheoryData
    {
        private readonly UserUpdateCommandHandler _handler;
        private readonly IUserRepository _repository;
        private readonly IJwtProvider _jwtProvide;
        private readonly IUnitOfWork _unitOfWork;
        private readonly User _user;
        private readonly Username _username;
        private readonly Email _email;
        private readonly PasswordHashed _password;
        private readonly UserUpdateCommandValidator _validator;

        public UserUpdateCommandTests()
        {
            _repository = Substitute.For<IUserRepository>();
            _jwtProvide = Substitute.For<IJwtProvider>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _username = Username.Create("username").Value;
            _email = Email.Create("str@ing").Value;
            _password = PasswordHashed.Create("password").Value;
            _user = User.Create(_email, _username, _password).Value;
            _validator = new();
            _handler = new UserUpdateCommandHandler(_jwtProvide, _repository, _unitOfWork);
            _repository.UpdateAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<User>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _user.Id), Arg.Any<CancellationToken>(), FetchMode.NoTracking)
                .Returns(_user);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _user.Id), Arg.Any<CancellationToken>(), FetchMode.NoTracking)
                .Returns(Result.Failure<User>(PersistenceErrors.Entity<User>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<User>>());
        }

        [Theory]
        [MemberData(nameof(InvalidNameCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, Name, _email.Value, _password.Value), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidNameCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, Name, _email.Value, _password.Value), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidEmailCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidEmailInput(string Email, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, _username.Value, Email, _password.Value), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidEmailCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidEmailInput(string Email)
        {
            //Act
            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, _username.Value, Email, _password.Value), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidPasswordCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidPasswordInput(string Password, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, _username.Value, _email.Value, Password), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidPasswordCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenPasswordInput(string Password)
        {
            //Act
            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, _username.Value, _email.Value, Password), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new UserUpdateCommand(Guid.Parse(id), _username.Value, _email.Value, _password.Value), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<User>.NotFound);
        }
        [Fact]
        public async Task Handle_Should_ReturnError_WhenAllParamsAreNull()
        {
            //Act
            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, null, null, null), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }
        [Fact]
        public async Task Handle_Should_ReturnError_WhenSaveIsFailer()
        {
            //Arrange
            _unitOfWork.SaveChangesAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<User>(PersistenceErrors.Entity<User>.NotFound));

            //Act
            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, _username.Value, _email.Value, _password.Value), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }

        [Fact]
        public void Should_Have_Error_When_Username_Is_Too_Long()
        {
            // Arrange
            var model = new UserUpdateCommand(
                _user.Id,
                new string('a', Username.MAX_LENGTH + 1), null, null
            );

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage(DomainErrors.Username.TooLong);
        }

        [Fact]
        public void Should_Have_Error_When_Username_Is_Too_Short()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                new string('a', Username.MIN_LENGTH - 1), null, null
            );

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage(DomainErrors.Username.TooShort);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Username_Is_Null()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                null, "statst@adsadasd", "paasdsaaads"
            );

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Username_Is_Valid()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                new string('a', Username.MIN_LENGTH), null, null
            );

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid_Format()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                null, "invalid-email", null
            );
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage(DomainErrors.Email.InvalidFormat);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Too_Long()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                null, $"{new string('a', Email.MAX_LENGTH + 10)}@test.com", null
            );

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage(DomainErrors.Email.TooLong);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Too_Short()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                null, "a@b.c", null
            );

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage(DomainErrors.Email.TooShort);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Null()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                "paasdsaaads", null, "paasdsaaads"
            );

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                null, "valid@example.com", null
            );

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Long()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                null, null, new string('a', PasswordHashed.MAX_LENGTH + 1)
            );

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage(DomainErrors.PasswordHashed.TooLong);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                null, null, new string('a', PasswordHashed.MIN_LENGTH - 1)
            );

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage(DomainErrors.PasswordHashed.TooShort);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Password_Is_Null()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                "paasdsaaads", "paasd@saaads", null
            );

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var model = new UserUpdateCommand(
                _user.Id,
                "validusername", "valid@example.com", "validpassword123"
            );

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}