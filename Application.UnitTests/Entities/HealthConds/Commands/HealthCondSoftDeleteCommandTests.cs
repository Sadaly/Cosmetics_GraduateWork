using Application.Entity.HealthConds.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.HealthConds.Commands
{
	public class HealthCondSoftDeleteCommandTests : TestsTheoryData
	{
		private readonly HealthCondSoftDeleteCommandHandler _handler;
		private readonly IHealthCondRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly Patient _patient;
		private readonly PatientCard _patientCard;
		private readonly HealthCondType _healthCondType;
		private readonly HealthCond _healthcond;

		public HealthCondSoftDeleteCommandTests()
		{
			_repository = Substitute.For<IHealthCondRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_patient = Patient.Create(Username.Create("Fullname")).Value;
			_patientCard = _patient.Card;
			_healthCondType = HealthCondType.Create(Title.Create("Title")).Value;
			_healthcond = HealthCond.Create(_patientCard, _healthCondType).Value;
			_handler = new HealthCondSoftDeleteCommandHandler(_repository, _unitOfWork);

			_repository.RemoveAsync(Arg.Any<Result<HealthCond>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<HealthCond>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _healthcond.Id), Arg.Any<CancellationToken>())
				.Returns(_healthcond);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _healthcond.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<HealthCond>(PersistenceErrors.Entity<HealthCond>.IsSoftDeleted));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<HealthCond>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<HealthCond>>());
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new HealthCondSoftDeleteCommand(_healthcond.Id), default);

			//Assert
			result.Value.Should().Be(_healthcond.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new HealthCondSoftDeleteCommand(Guid.Parse(id)), default);

			result.Error.Should().Be(PersistenceErrors.Entity<HealthCond>.IsSoftDeleted);
		}
	}
}