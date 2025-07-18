using Application.Entity.AgeChangeTypes.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.AgeChangeTypes.Commands
{
    public class AgeChangeTypeSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly AgeChangeTypeSoftDeleteCommandHandler _handler;
        private readonly IAgeChangeTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AgeChangeType _agechangetype;

        public AgeChangeTypeSoftDeleteCommandTests()
        {
            _repository = Substitute.For<IAgeChangeTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _agechangetype = AgeChangeType.Create(Title.Create("Fullname").Value).Value;

            _handler = new AgeChangeTypeSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<AgeChangeType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<AgeChangeType>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _agechangetype.Id), Arg.Any<CancellationToken>())
                .Returns(_agechangetype);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _agechangetype.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<AgeChangeType>(PersistenceErrors.Entity<AgeChangeType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChangeType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<AgeChangeType>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new AgeChangeTypeSoftDeleteCommand(_agechangetype.Id), default);

            //Assert
            result.Value.Should().Be(_agechangetype.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new AgeChangeTypeSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<AgeChangeType>.NotFound);
        }
    }
}