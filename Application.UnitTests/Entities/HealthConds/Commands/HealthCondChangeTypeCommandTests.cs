using Application.Entity.HealthConds.Commands.ChangeType;
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
    public class HealthCondChangeTypeCommandTests : TestsTheoryData
    {
        private readonly HealthCondChangeTypeCommandHandler _handler;
        private readonly IHealthCondRepository _repository;
        private readonly IHealthCondTypeRepository _typeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly HealthCondType _healthCondType;
        private readonly HealthCondType _healthCondTypeUpdated;
        private readonly HealthCond _healthcond;


        public HealthCondChangeTypeCommandTests()
        {
            _repository = Substitute.For<IHealthCondRepository>();
            _typeRepository = Substitute.For<IHealthCondTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _healthCondType = HealthCondType.Create(Title.Create("Title")).Value;
            _healthCondTypeUpdated = HealthCondType.Create(Title.Create("Updated")).Value;
            _healthcond = HealthCond.Create(_patientCard, _healthCondType).Value;
            _handler = new HealthCondChangeTypeCommandHandler(_repository, _typeRepository, _unitOfWork);

            _repository.UpdateAsync(Arg.Any<Result<HealthCond>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCond>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _healthcond.Id), Arg.Any<CancellationToken>())
                .Returns(_healthcond);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _healthcond.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<HealthCond>(PersistenceErrors.Entity<HealthCond>.NotFound));

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _healthCondTypeUpdated.Id), Arg.Any<CancellationToken>())
                .Returns(_healthCondTypeUpdated);

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != _healthCondTypeUpdated.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<HealthCondType>(PersistenceErrors.Entity<HealthCondType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<HealthCond>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCond>>());
        }

        [Theory]
        [MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnFailure_WhenInvalidGuids(string HealthCondId, string newTypeId)
        {
            //Act
            var result = await _handler.Handle(new HealthCondChangeTypeCommand(Guid.Parse(HealthCondId), Guid.Parse(newTypeId)), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidGuids()
        {
            //Act
            var result = await _handler.Handle(new HealthCondChangeTypeCommand(_healthcond.Id, _healthCondTypeUpdated.Id), default);

            //Assert
            result.Value.Should().Be(_healthcond.Id);
        }
    }
}