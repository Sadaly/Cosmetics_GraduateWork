using Application.Entity.AgeChangeTypes.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.AgeChangeTypes.Commands
{
	public class AgeChangeTypeUpdateCommandTests : TestsTheoryData
	{
		private readonly AgeChangeTypeUpdateCommandHandler _handler;
		private readonly IAgeChangeTypeRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly AgeChangeType _agechangetype;

		public AgeChangeTypeUpdateCommandTests()
		{
			_repository = Substitute.For<IAgeChangeTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_agechangetype = AgeChangeType.Create(Title.Create("Fullname").Value).Value;

			_handler = new AgeChangeTypeUpdateCommandHandler(_repository, _unitOfWork);
			_repository.UpdateAsync(Arg.Any<Result<AgeChangeType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<AgeChangeType>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _agechangetype.Id), Arg.Any<CancellationToken>())
				.Returns(_agechangetype);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _agechangetype.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<AgeChangeType>(PersistenceErrors.Entity<AgeChangeType>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChangeType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<AgeChangeType>>());
		}

		[Theory]
		[MemberData(nameof(InvalidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
		{
			//Act
			var result = await _handler.Handle(new AgeChangeTypeUpdateCommand(_agechangetype.Id, Name), default);

			//Assert
			result.Error.Code.Should().Be(expectedErrorCode);
		}

		[Theory]
		[MemberData(nameof(ValidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
		{
			//Act
			var result = await _handler.Handle(new AgeChangeTypeUpdateCommand(_agechangetype.Id, Name), default);

			//Assert
			result.Value.Should().Be(_agechangetype.Id);
			_agechangetype.Title.Should().Be(Title.Create(Name).Value);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new AgeChangeTypeUpdateCommand(Guid.Parse(id), _agechangetype.Title.Value), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<AgeChangeType>.NotFound);
		}

		[Fact]
		public async Task Handle_Should_ReturnError_WhenSaveIsFailer()
		{
			//Arrange
			_unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChangeType>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<AgeChangeType>(PersistenceErrors.Entity<AgeChangeType>.NotFound));

			//Act
			var result = await _handler.Handle(new AgeChangeTypeUpdateCommand(_agechangetype.Id, _agechangetype.Title.Value), default);

			//Assert
			result.IsFailure.Should().Be(true);
		}
	}
}