using Application.Entity.AgeChanges.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.AgeChanges.Commands
{
    public class AgeChangeCreateCommandTests : TestsTheoryData
    {
        private readonly AgeChangeCreateCommandHandler _handler;
        private readonly IAgeChangeRepository _repository;
        private readonly IAgeChangeTypeRepository _typeRepository;
        private readonly IPatientCardRepository _patientCardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly AgeChangeType _ageChangeType;

        public AgeChangeCreateCommandTests()
        {
            _repository = Substitute.For<IAgeChangeRepository>();
            _typeRepository = Substitute.For<IAgeChangeTypeRepository>();
            _patientCardRepository = Substitute.For<IPatientCardRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _ageChangeType = AgeChangeType.Create(Title.Create("Title")).Value;

            _handler = new AgeChangeCreateCommandHandler(_repository, _typeRepository, _patientCardRepository, _unitOfWork);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_patientCard);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<PatientCard>(PersistenceErrors.Entity<PatientCard>.NotFound));

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_ageChangeType);

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<AgeChangeType>(PersistenceErrors.Entity<AgeChangeType>.NotFound));

            _repository.AddAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<AgeChange>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<AgeChange>>());
        }

        [Theory]
        [MemberData(nameof(ValidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidGuidsEnteredInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new AgeChangeCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnFailure_WhenInvalidNameInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new AgeChangeCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }
    }
}