using Application.Entity.SkinFeatureTypes.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinFeatureTypes.Commands
{
    public class SkinFeatureTypeSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly SkinFeatureTypeSoftDeleteCommandHandler _handler;
        private readonly ISkinFeatureTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SkinFeatureType _skinfeaturetype;

        public SkinFeatureTypeSoftDeleteCommandTests()
        {
            _repository = Substitute.For<ISkinFeatureTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _skinfeaturetype = SkinFeatureType.Create(Title.Create("Fullname").Value).Value;

            _handler = new SkinFeatureTypeSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<SkinFeatureType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinFeatureType>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _skinfeaturetype.Id), Arg.Any<CancellationToken>())
                .Returns(_skinfeaturetype);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _skinfeaturetype.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinFeatureType>(PersistenceErrors.Entity<SkinFeatureType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinFeatureType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinFeatureType>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureTypeSoftDeleteCommand(_skinfeaturetype.Id), default);

            //Assert
            result.Value.Should().Be(_skinfeaturetype.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureTypeSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<SkinFeatureType>.NotFound);
        }
    }
}