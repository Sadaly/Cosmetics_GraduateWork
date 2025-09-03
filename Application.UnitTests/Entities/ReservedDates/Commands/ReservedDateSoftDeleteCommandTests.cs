using Application.Entity.ReservedDates.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ReservedDates.Commands
{
	public class ReservedDateSoftDeleteCommandTests : TestsTheoryData
	{
		private readonly ReservedDateSoftDeleteCommandHandler _handler;
		private readonly IReservedDateRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ReservedDate _reserveddate;

		public ReservedDateSoftDeleteCommandTests()
		{
			_repository = Substitute.For<IReservedDateRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_reserveddate = ReservedDate.Create(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), ReservedDateType.None).Value;

			_handler = new ReservedDateSoftDeleteCommandHandler(_repository, _unitOfWork);

			_repository.RemoveAsync(Arg.Any<Result<ReservedDate>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<ReservedDate>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _reserveddate.Id), Arg.Any<CancellationToken>())
				.Returns(_reserveddate);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _reserveddate.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<ReservedDate>(PersistenceErrors.Entity<ReservedDate>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<ReservedDate>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<ReservedDate>>());
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new ReservedDateSoftDeleteCommand(_reserveddate.Id), default);

			//Assert
			result.Value.Should().Be(_reserveddate.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new ReservedDateSoftDeleteCommand(Guid.Parse(id)), default);

			result.Error.Should().Be(PersistenceErrors.Entity<ReservedDate>.NotFound);
		}
	}
}