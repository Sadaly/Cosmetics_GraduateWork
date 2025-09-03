using Application.Entity.ProcedureTypes.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ProcedureTypes.Commands
{
	public class ProcedureTypeCreateCommandTests : TestsTheoryData
	{
		private readonly ProcedureTypeCreateCommandHandler _handler;
		private readonly IProcedureTypeRepository _repository;
		private readonly IUnitOfWork _unitOfWork;

		public ProcedureTypeCreateCommandTests()
		{
			_repository = Substitute.For<IProcedureTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();

			_handler = new ProcedureTypeCreateCommandHandler(_repository, _unitOfWork);

			_repository.AddAsync(Arg.Any<Result<ProcedureType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<ProcedureType>>());

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<ProcedureType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<ProcedureType>>());
		}

		[Theory]
		[MemberData(nameof(InvalidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Title, string expectedErrorCode)
		{
			//Act
			var result = await _handler.Handle(new ProcedureTypeCreateCommand(Title, "", 0), default);

			//Assert
			result.Error.Code.Should().Be(expectedErrorCode);
		}

		[Theory]
		[MemberData(nameof(ValidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidTitleInput(string Title)
		{
			//Act
			var result = await _handler.Handle(new ProcedureTypeCreateCommand(Title, "", 0), default);

			//Assert
			result.IsSuccess.Should().Be(true);
		}
	}
}