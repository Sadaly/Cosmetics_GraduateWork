using Application.Entity.HealthCondTypes.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.HealthCondTypes.Commands
{
    public class HealthCondTypeSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly HealthCondTypeSoftDeleteCommandHandler _handler;
        private readonly IHealthCondTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HealthCondType _healthcondtype;

        public HealthCondTypeSoftDeleteCommandTests()
        {
            _repository = Substitute.For<IHealthCondTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _healthcondtype = HealthCondType.Create(Title.Create("Fullname").Value).Value;

            _handler = new HealthCondTypeSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<HealthCondType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCondType>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _healthcondtype.Id), Arg.Any<CancellationToken>())
                .Returns(_healthcondtype);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _healthcondtype.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<HealthCondType>(PersistenceErrors.Entity<HealthCondType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<HealthCondType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCondType>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new HealthCondTypeSoftDeleteCommand(_healthcondtype.Id), default);

            //Assert
            result.Value.Should().Be(_healthcondtype.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new HealthCondTypeSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<HealthCondType>.NotFound);
        }
    }
}