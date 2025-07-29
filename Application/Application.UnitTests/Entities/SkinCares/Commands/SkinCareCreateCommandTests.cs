using Application.Entity.SkinCares.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinCares.Commands
{
    public class SkinCareCreateCommandTests : TestsTheoryData
    {
        private readonly SkinCareCreateCommandHandler _handler;
        private readonly ISkinCareRepository _repository;
        private readonly ISkinCareTypeRepository _typeRepository;
        private readonly IPatientCardRepository _patientCardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly SkinCareType _skinCareType;

        public SkinCareCreateCommandTests()
        {
            _repository = Substitute.For<ISkinCareRepository>();
            _typeRepository = Substitute.For<ISkinCareTypeRepository>();
            _patientCardRepository = Substitute.For<IPatientCardRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _skinCareType = SkinCareType.Create(Title.Create("Title")).Value;

            _handler = new SkinCareCreateCommandHandler(_repository, _typeRepository, _patientCardRepository, _unitOfWork);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_patientCard);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<PatientCard>(PersistenceErrors.Entity<PatientCard>.NotFound));

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_skinCareType);

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinCareType>(PersistenceErrors.Entity<SkinCareType>.NotFound));

            _repository.AddAsync(Arg.Any<Result<SkinCare>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinCare>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinCare>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinCare>>());
        }

        [Theory]
        [MemberData(nameof(ValidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidGuidsEnteredInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new SkinCareCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnFailure_WhenInvalidNameInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new SkinCareCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }
    }
}