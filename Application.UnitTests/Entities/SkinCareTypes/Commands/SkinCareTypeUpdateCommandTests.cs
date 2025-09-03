using Application.Entity.SkinCareTypes.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinCareTypes.Commands
{
	public class SkinCareTypeUpdateCommandTests : TestsTheoryData
	{
		private readonly SkinCareTypeUpdateCommandHandler _handler;
		private readonly ISkinCareTypeRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly SkinCareType _skincaretype;

		public SkinCareTypeUpdateCommandTests()
		{
			_repository = Substitute.For<ISkinCareTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_skincaretype = SkinCareType.Create(Title.Create("Fullname").Value).Value;

			_handler = new SkinCareTypeUpdateCommandHandler(_repository, _unitOfWork);
			_repository.UpdateAsync(Arg.Any<Result<SkinCareType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinCareType>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _skincaretype.Id), Arg.Any<CancellationToken>())
				.Returns(_skincaretype);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _skincaretype.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinCareType>(PersistenceErrors.Entity<SkinCareType>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinCareType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinCareType>>());
		}

		[Theory]
		[MemberData(nameof(InvalidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
		{
			//Act
			var result = await _handler.Handle(new SkinCareTypeUpdateCommand(_skincaretype.Id, Name), default);

			//Assert
			result.Error.Code.Should().Be(expectedErrorCode);
		}

		[Theory]
		[MemberData(nameof(ValidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
		{
			//Act
			var result = await _handler.Handle(new SkinCareTypeUpdateCommand(_skincaretype.Id, Name), default);

			//Assert
			result.Value.Should().Be(_skincaretype.Id);
			_skincaretype.Title.Should().Be(Title.Create(Name).Value);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new SkinCareTypeUpdateCommand(Guid.Parse(id), _skincaretype.Title.Value), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<SkinCareType>.NotFound);
		}
		[Fact]
		public async Task Handle_Should_ReturnError_WhenSaveIsFailer()
		{
			//Arrange
			_unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinCareType>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinCareType>(PersistenceErrors.Entity<SkinCareType>.NotFound));

			//Act
			var result = await _handler.Handle(new SkinCareTypeUpdateCommand(_skincaretype.Id, _skincaretype.Title.Value), default);

			//Assert
			result.IsFailure.Should().Be(true);
		}
	}
}