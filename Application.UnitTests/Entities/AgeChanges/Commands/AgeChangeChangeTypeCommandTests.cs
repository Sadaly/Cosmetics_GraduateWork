using Application.Entity.AgeChanges.Commands.ChangeType;
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
    public class AgeChangeChangeTypeCommandTests : TestsTheoryData
    {
        private readonly AgeChangeChangeTypeCommandHandler _handler;
        private readonly IAgeChangeRepository _repository;
        private readonly IAgeChangeTypeRepository _typeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly AgeChangeType _ageChangeType;
        private readonly AgeChangeType _ageChangeTypeUpdated;
        private readonly AgeChange _agechange;


        public AgeChangeChangeTypeCommandTests()
        {
            _repository = Substitute.For<IAgeChangeRepository>();
            _typeRepository = Substitute.For<IAgeChangeTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _ageChangeType = AgeChangeType.Create(Title.Create("Title")).Value;
            _ageChangeTypeUpdated = AgeChangeType.Create(Title.Create("Updated")).Value;
            _agechange = AgeChange.Create(_patientCard, _ageChangeType).Value;
            _handler = new AgeChangeChangeTypeCommandHandler(_repository, _typeRepository, _unitOfWork);

            _repository.UpdateAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<AgeChange>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _agechange.Id), Arg.Any<CancellationToken>())
                .Returns(_agechange);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _agechange.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<AgeChange>(PersistenceErrors.Entity<AgeChange>.NotFound));

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _ageChangeTypeUpdated.Id), Arg.Any<CancellationToken>())
                .Returns(_ageChangeTypeUpdated);

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != _ageChangeTypeUpdated.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<AgeChangeType>(PersistenceErrors.Entity<AgeChangeType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<AgeChange>>());
        }

        [Theory]
        [MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnFailure_WhenInvalidGuids(string AgeChangeId, string newTypeId)
        {
            //Act
            var result = await _handler.Handle(new AgeChangeChangeTypeCommand(Guid.Parse(AgeChangeId), Guid.Parse(newTypeId)), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidGuids()
        {
            //Act
            var result = await _handler.Handle(new AgeChangeChangeTypeCommand(_agechange.Id, _ageChangeTypeUpdated.Id), default);

            //Assert
            result.Value.Should().Be(_agechange.Id);
        }
    }
}