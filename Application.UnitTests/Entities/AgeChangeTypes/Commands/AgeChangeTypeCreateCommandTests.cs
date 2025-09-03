using Application.Entity.AgeChangeTypes.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.AgeChangeTypes.Commands
{
	public class AgeChangeTypeCreateCommandTests : TestsTheoryData
	{
		private readonly AgeChangeTypeCreateCommandHandler _handler;
		private readonly IAgeChangeTypeRepository _repository;
		private readonly IUnitOfWork _unitOfWork;

		public AgeChangeTypeCreateCommandTests()
		{
			_repository = Substitute.For<IAgeChangeTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();

			_handler = new AgeChangeTypeCreateCommandHandler(_repository, _unitOfWork);

			_repository.AddAsync(Arg.Any<Result<AgeChangeType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<AgeChangeType>>());

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChangeType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<AgeChangeType>>());
		}

		[Theory]
		[MemberData(nameof(InvalidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Title, string expectedErrorCode)
		{
			//Act
			var result = await _handler.Handle(new AgeChangeTypeCreateCommand(Title), default);

			//Assert
			result.Error.Code.Should().Be(expectedErrorCode);
		}

		[Theory]
		[MemberData(nameof(ValidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidTitleInput(string Title)
		{
			//Act
			var result = await _handler.Handle(new AgeChangeTypeCreateCommand(Title), default);

			//Assert
			result.IsSuccess.Should().Be(true);
		}
	}
}