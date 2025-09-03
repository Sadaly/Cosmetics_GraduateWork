using Application.Entity.ExternalProcedureRecordTypes.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ExternalProcedureRecordTypes.Commands
{
	public class ExternalProcedureRecordTypeSoftDeleteCommandTests : TestsTheoryData
	{
		private readonly ExternalProcedureRecordTypeSoftDeleteCommandHandler _handler;
		private readonly IExternalProcedureRecordTypeRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ExternalProcedureRecordType _externalprocedurerecordtype;

		public ExternalProcedureRecordTypeSoftDeleteCommandTests()
		{
			_repository = Substitute.For<IExternalProcedureRecordTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_externalprocedurerecordtype = ExternalProcedureRecordType.Create(Title.Create("Fullname").Value).Value;

			_handler = new ExternalProcedureRecordTypeSoftDeleteCommandHandler(_repository, _unitOfWork);

			_repository.RemoveAsync(Arg.Any<Result<ExternalProcedureRecordType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<ExternalProcedureRecordType>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _externalprocedurerecordtype.Id), Arg.Any<CancellationToken>())
				.Returns(_externalprocedurerecordtype);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _externalprocedurerecordtype.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<ExternalProcedureRecordType>(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<ExternalProcedureRecordType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<ExternalProcedureRecordType>>());
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new ExternalProcedureRecordTypeSoftDeleteCommand(_externalprocedurerecordtype.Id), default);

			//Assert
			result.Value.Should().Be(_externalprocedurerecordtype.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new ExternalProcedureRecordTypeSoftDeleteCommand(Guid.Parse(id)), default);

			result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound);
		}
	}
}