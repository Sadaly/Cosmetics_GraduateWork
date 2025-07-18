using Application.Entity.AgeChanges.Commands.SoftDelete;
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
    public class AgeChangeSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly AgeChangeSoftDeleteCommandHandler _handler;
        private readonly IAgeChangeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly AgeChangeType _ageChangeType;
        private readonly AgeChange _agechange;

        public AgeChangeSoftDeleteCommandTests()
        {
            _repository = Substitute.For<IAgeChangeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _ageChangeType = AgeChangeType.Create(Title.Create("Title")).Value;
            _agechange = AgeChange.Create(_patientCard, _ageChangeType).Value;
            _handler = new AgeChangeSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<AgeChange>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _agechange.Id), Arg.Any<CancellationToken>())
                .Returns(_agechange);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _agechange.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<AgeChange>(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<AgeChange>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new AgeChangeSoftDeleteCommand(_agechange.Id), default);

            //Assert
            result.Value.Should().Be(_agechange.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new AgeChangeSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted);
        }
    }
}