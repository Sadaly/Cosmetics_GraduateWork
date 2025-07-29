using Application.Entity.SkinCareTypes.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinCareTypes.Commands
{
    public class SkinCareTypeSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly SkinCareTypeSoftDeleteCommandHandler _handler;
        private readonly ISkinCareTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SkinCareType _skincaretype;

        public SkinCareTypeSoftDeleteCommandTests()
        {
            _repository = Substitute.For<ISkinCareTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _skincaretype = SkinCareType.Create(Title.Create("Fullname").Value).Value;

            _handler = new SkinCareTypeSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<SkinCareType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinCareType>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _skincaretype.Id), Arg.Any<CancellationToken>())
                .Returns(_skincaretype);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _skincaretype.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinCareType>(PersistenceErrors.Entity<SkinCareType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinCareType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinCareType>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinCareTypeSoftDeleteCommand(_skincaretype.Id), default);

            //Assert
            result.Value.Should().Be(_skincaretype.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new SkinCareTypeSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<SkinCareType>.NotFound);
        }
    }
}