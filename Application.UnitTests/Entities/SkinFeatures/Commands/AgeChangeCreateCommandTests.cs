using Application.Entity.SkinFeatures.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinFeatures.Commands
{
    public class SkinFeatureCreateCommandTests : TestsTheoryData
    {
        private readonly SkinFeatureCreateCommandHandler _handler;
        private readonly ISkinFeatureRepository _repository;
        private readonly ISkinFeatureTypeRepository _typeRepository;
        private readonly IPatientCardRepository _patientCardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly SkinFeatureType _skinFeatureType;

        public SkinFeatureCreateCommandTests()
        {
            _repository = Substitute.For<ISkinFeatureRepository>();
            _typeRepository = Substitute.For<ISkinFeatureTypeRepository>();
            _patientCardRepository = Substitute.For<IPatientCardRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _skinFeatureType = SkinFeatureType.Create(Title.Create("Title")).Value;

            _handler = new SkinFeatureCreateCommandHandler(_repository, _typeRepository, _patientCardRepository, _unitOfWork);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_patientCard);

            _patientCardRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<PatientCard>(PersistenceErrors.Entity<PatientCard>.NotFound));

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(_skinFeatureType);

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == Guid.Empty), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinFeatureType>(PersistenceErrors.Entity<SkinFeatureType>.NotFound));

            _repository.AddAsync(Arg.Any<Result<SkinFeature>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinFeature>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinFeature>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinFeature>>());
        }

        [Theory]
        [MemberData(nameof(ValidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidGuidsEnteredInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnFailure_WhenInvalidNameInput(string patientCardId, string typeId)
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureCreateCommand(Guid.Parse(patientCardId), Guid.Parse(typeId)), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }
    }
}