using Application.Entity.ExternalProcedureRecords.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ExternalProcedureRecords.Commands
{
    public class ExternalProcedureRecordCreateCommandTests : TestsTheoryData
    {
        private readonly ExternalProcedureRecordCreateCommandHandler _handler;
        private readonly IExternalProcedureRecordRepository _repository;
        private readonly IExternalProcedureRecordTypeRepository _typeRepository;
        private readonly IPatientCardRepository _patientCardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly ExternalProcedureRecordType _externalProcedureRecordType;

        public ExternalProcedureRecordCreateCommandTests()
        {
            _repository = Substitute.For<IExternalProcedureRecordRepository>();
            _typeRepository = Substitute.For<IExternalProcedureRecordTypeRepository>();
            _patientCardRepository = Substitute.For<IPatientCardRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _externalProcedureRecordType = ExternalProcedureRecordType.Create(Title.Create("Title")).Value;

            _handler = new ExternalProcedureRecordCreateCommandHandler(_repository, _typeRepository, _patientCardRepository, _unitOfWork);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_patientCard);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<PatientCard>(PersistenceErrors.Entity<PatientCard>.NotFound));

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_externalProcedureRecordType);

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ExternalProcedureRecordType>(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound));

            _repository.AddAsync(Arg.Any<Result<ExternalProcedureRecord>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecord>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ExternalProcedureRecord>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecord>>());
        }

        [Theory]
        [MemberData(nameof(ValidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidGuidsEnteredInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId), "anydate"), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnFailure_WhenInvalidNameInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId), "anydate"), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }
    }
}