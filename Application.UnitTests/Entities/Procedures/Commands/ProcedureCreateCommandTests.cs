using Application.Abstractions;
using Application.Entity.Procedures.Commands.Create;
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
	public class ProcedureCreateCommandTests : TestsTheoryData
	{
		private readonly ProcedureCreateCommandHandler _handler;
		private readonly IProcedureRepository _repository;
		private readonly IDoctorRepository _doctorRepository;
		private readonly IProcedureTypeRepository _typeRepository;
		private readonly IPatientCardRepository _patientCardRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IProcedureScheduleService _procedureScheduleService;
		private readonly Patient _patient;
		private readonly PatientCard _patientCard;
		private readonly ProcedureType _procedureType;

		public ProcedureCreateCommandTests()
		{
			_repository = Substitute.For<IProcedureRepository>();
			_doctorRepository = Substitute.For<IDoctorRepository>();
			_typeRepository = Substitute.For<IProcedureTypeRepository>();
			_patientCardRepository = Substitute.For<IPatientCardRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_procedureScheduleService = Substitute.For<IProcedureScheduleService>();
			_patient = Patient.Create(Username.Create("Fullname")).Value;
			_patientCard = _patient.Card;
			_procedureType = ProcedureType.Create(Title.Create("Title"), "", 0, 0).Value;

			_handler = new ProcedureCreateCommandHandler(_procedureScheduleService, _repository, _typeRepository, _doctorRepository, _patientCardRepository, _unitOfWork);

			_patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
				.Returns(_patientCard);

			_patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<PatientCard>(PersistenceErrors.Entity<PatientCard>.NotFound));

			_typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
				.Returns(_procedureType);

			_typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<ProcedureType>(PersistenceErrors.Entity<ProcedureType>.NotFound));

			_repository.AddAsync(Arg.Any<Result<Procedure>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<Procedure>>());

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<Procedure>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<Procedure>>());
		}

		[Theory]
		[MemberData(nameof(ValidEntityWithTypeGuidsTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidGuidsEnteredInput(string patientCardId, string typeId)
		{
			//Act
			var result = await _handler.Handle(new ProcedureCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

			//Assert
			result.IsSuccess.Should().Be(true);
		}

		[Theory]
		[MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
		public async Task Handle_Should_ReturnFailure_WhenInvalidNameInput(string patientCardId, string typeId)
		{
			//Act
			var result = await _handler.Handle(new ProcedureCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

			//Assert
			result.IsFailure.Should().Be(true);
		}
	}
}