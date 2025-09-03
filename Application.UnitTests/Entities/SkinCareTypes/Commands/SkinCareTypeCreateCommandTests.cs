using Application.Entity.SkinCareTypes.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinCareTypes.Commands
{
	public class SkinCareTypeCreateCommandTests : TestsTheoryData
	{
		private readonly SkinCareTypeCreateCommandHandler _handler;
		private readonly ISkinCareTypeRepository _repository;
		private readonly IUnitOfWork _unitOfWork;

		public SkinCareTypeCreateCommandTests()
		{
			_repository = Substitute.For<ISkinCareTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();

			_handler = new SkinCareTypeCreateCommandHandler(_repository, _unitOfWork);

			_repository.AddAsync(Arg.Any<Result<SkinCareType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinCareType>>());

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinCareType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinCareType>>());
		}

		[Theory]
		[MemberData(nameof(InvalidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Title, string expectedErrorCode)
		{
			//Act
			var result = await _handler.Handle(new SkinCareTypeCreateCommand(Title), default);

			//Assert
			result.Error.Code.Should().Be(expectedErrorCode);
		}

		[Theory]
		[MemberData(nameof(ValidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidTitleInput(string Title)
		{
			//Act
			var result = await _handler.Handle(new SkinCareTypeCreateCommand(Title), default);

			//Assert
			result.IsSuccess.Should().Be(true);
		}
	}
}