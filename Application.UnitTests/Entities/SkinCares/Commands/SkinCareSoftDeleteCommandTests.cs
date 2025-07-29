using Application.Entity.SkinCares.Commands.SoftDelete;
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
    public class SkinCareSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly SkinCareSoftDeleteCommandHandler _handler;
        private readonly ISkinCareRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly SkinCareType _skinCareType;
        private readonly SkinCare _skincare;

        public SkinCareSoftDeleteCommandTests()
        {
            _repository = Substitute.For<ISkinCareRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _skinCareType = SkinCareType.Create(Title.Create("Title")).Value;
            _skincare = SkinCare.Create(_patientCard, _skinCareType).Value;
            _handler = new SkinCareSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<SkinCare>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinCare>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _skincare.Id), Arg.Any<CancellationToken>())
                .Returns(_skincare);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _skincare.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinCare>(PersistenceErrors.Entity<SkinCare>.IsSoftDeleted));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinCare>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinCare>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinCareSoftDeleteCommand(_skincare.Id), default);

            //Assert
            result.Value.Should().Be(_skincare.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new SkinCareSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<SkinCare>.IsSoftDeleted);
        }
    }
}