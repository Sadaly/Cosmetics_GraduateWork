using Application.Entity.SkinFeatureTypes.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinFeatureTypes.Commands
{
    public class SkinFeatureTypeCreateCommandTests : TestsTheoryData
    {
        private readonly SkinFeatureTypeCreateCommandHandler _handler;
        private readonly ISkinFeatureTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SkinFeatureTypeCreateCommandTests()
        {
            _repository = Substitute.For<ISkinFeatureTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _handler = new SkinFeatureTypeCreateCommandHandler(_repository, _unitOfWork);

            _repository.AddAsync(Arg.Any<Result<SkinFeatureType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinFeatureType>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinFeatureType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<SkinFeatureType>>());
        }

        [Theory]
        [MemberData(nameof(InvalidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Title, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureTypeCreateCommand(Title), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidTitleInput(string Title)
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureTypeCreateCommand(Title), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }
    }
}