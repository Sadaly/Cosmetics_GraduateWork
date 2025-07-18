using Application.Entity.SkinFeatures.Commands.SoftDelete;
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
    public class SkinFeatureSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly SkinFeatureSoftDeleteCommandHandler _handler;
        private readonly ISkinFeatureRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly SkinFeatureType _skinFeatureType;
        private readonly SkinFeature _skinfeature;

        public SkinFeatureSoftDeleteCommandTests()
        {
            _repository = Substitute.For<ISkinFeatureRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _skinFeatureType = SkinFeatureType.Create(Title.Create("Title")).Value;
            _skinfeature = SkinFeature.Create(_patientCard, _skinFeatureType).Value;
            _handler = new SkinFeatureSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<SkinFeature>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinFeature>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _skinfeature.Id), Arg.Any<CancellationToken>())
                .Returns(_skinfeature);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _skinfeature.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinFeature>(PersistenceErrors.Entity<SkinFeature>.IsSoftDeleted));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinFeature>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinFeature>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureSoftDeleteCommand(_skinfeature.Id), default);

            //Assert
            result.Value.Should().Be(_skinfeature.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<SkinFeature>.IsSoftDeleted);
        }
    }
}