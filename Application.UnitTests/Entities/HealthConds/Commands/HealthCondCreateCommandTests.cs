using Application.Entity.HealthConds.Commands.Create;
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
    public class HealthCondCreateCommandTests : TestsTheoryData
    {
        private readonly HealthCondCreateCommandHandler _handler;
        private readonly IHealthCondRepository _repository;
        private readonly IHealthCondTypeRepository _typeRepository;
        private readonly IPatientCardRepository _patientCardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly HealthCondType _healthCondType;

        public HealthCondCreateCommandTests()
        {
            _repository = Substitute.For<IHealthCondRepository>();
            _typeRepository = Substitute.For<IHealthCondTypeRepository>();
            _patientCardRepository = Substitute.For<IPatientCardRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _healthCondType = HealthCondType.Create(Title.Create("Title")).Value;

            _handler = new HealthCondCreateCommandHandler(_repository, _typeRepository, _patientCardRepository, _unitOfWork);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_patientCard);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<PatientCard>(PersistenceErrors.Entity<PatientCard>.NotFound));

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_healthCondType);

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<HealthCondType>(PersistenceErrors.Entity<HealthCondType>.NotFound));

            _repository.AddAsync(Arg.Any<Result<HealthCond>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCond>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<HealthCond>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCond>>());
        }

        [Theory]
        [MemberData(nameof(ValidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidGuidsEnteredInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new HealthCondCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnFailure_WhenInvalidNameInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new HealthCondCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }
    }
}