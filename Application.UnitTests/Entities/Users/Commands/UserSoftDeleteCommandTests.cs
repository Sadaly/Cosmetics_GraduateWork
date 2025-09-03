using Application.Entity.Users.Commands.SoftDelete;
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
	public class UserSoftDeleteCommandTests : TestsTheoryData
	{
		private readonly UserSoftDeleteCommandHandler _handler;
		private readonly IUserRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly User _user;
		private readonly Username _username;
		private readonly Email _email;
		private readonly PasswordHashed _password;

		public UserSoftDeleteCommandTests()
		{
			_repository = Substitute.For<IUserRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_username = Username.Create("username").Value;
			_email = Email.Create("str@ing").Value;
			_password = PasswordHashed.Create("password").Value;
			_user = User.Create(_email, _username, _password).Value;

			_handler = new UserSoftDeleteCommandHandler(_repository, _unitOfWork);

			_repository.RemoveAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<User>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _user.Id), Arg.Any<CancellationToken>())
				.Returns(_user);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _user.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<User>(PersistenceErrors.Entity<User>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<User>>());
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new UserSoftDeleteCommand(_user.Id), default);

			//Assert
			result.Value.Should().Be(_user.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new UserSoftDeleteCommand(Guid.Parse(id)), default);

			result.Error.Should().Be(PersistenceErrors.Entity<User>.NotFound);
		}
	}
}