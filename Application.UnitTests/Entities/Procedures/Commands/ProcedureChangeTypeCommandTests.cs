using Application.Entity.Procedures.Commands.ChangeType;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Procedures.Commands
{
	public class ProcedureChangeTypeCommandTests : TestsTheoryData
	{
		private readonly ProcedureChangeTypeCommandHandler _handler;
		private readonly IProcedureRepository _repository;
		private readonly IProcedureTypeRepository _typeRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly Patient _patient;
		private readonly PatientCard _patientCard;
		private readonly ProcedureType _procedureType;
		private readonly ProcedureType _procedureTypeUpdated;
		private readonly Procedure _procedure;


		public ProcedureChangeTypeCommandTests()
		{
			_repository = Substitute.For<IProcedureRepository>();
			_typeRepository = Substitute.For<IProcedureTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_patient = Patient.Create(Username.Create("Fullname")).Value;
			_patientCard = _patient.Card;
			_procedureType = ProcedureType.Create(Title.Create("Title"), "", 0, 0).Value;
			_procedureTypeUpdated = ProcedureType.Create(Title.Create("Updated"), "", 0, 0).Value;
			_procedure = Procedure.Create(_patientCard, _procedureType, _procedureType.StandartDuration, _procedureType.StandartPrice).Value;
			_handler = new ProcedureChangeTypeCommandHandler(_repository, _typeRepository, _unitOfWork);

			_repository.UpdateAsync(Arg.Any<Result<Procedure>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<Procedure>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _procedure.Id), Arg.Any<CancellationToken>())
				.Returns(_procedure);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _procedure.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<Procedure>(PersistenceErrors.Entity<Procedure>.NotFound));

			_typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _procedureTypeUpdated.Id), Arg.Any<CancellationToken>())
				.Returns(_procedureTypeUpdated);

			_typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != _procedureTypeUpdated.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<ProcedureType>(PersistenceErrors.Entity<ProcedureType>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<Procedure>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<Procedure>>());
		}

		[Theory]
		[MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
		public async Task Handle_Should_ReturnFailure_WhenInvalidGuids(string ProcedureId, string newTypeId)
		{
			//Act
			var result = await _handler.Handle(new ProcedureChangeTypeCommand(Guid.Parse(ProcedureId), Guid.Parse(newTypeId)), default);

			//Assert
			result.IsFailure.Should().Be(true);
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidGuids()
		{
			//Act
			var result = await _handler.Handle(new ProcedureChangeTypeCommand(_procedure.Id, _procedureTypeUpdated.Id), default);

			//Assert
			result.Value.Should().Be(_procedure.Id);
		}
	}
}