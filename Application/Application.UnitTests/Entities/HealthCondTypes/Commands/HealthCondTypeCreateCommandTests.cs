using Application.Entity.HealthCondTypes.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.HealthCondTypes.Commands
{
    public class HealthCondTypeCreateCommandTests : TestsTheoryData
    {
        private readonly HealthCondTypeCreateCommandHandler _handler;
        private readonly IHealthCondTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public HealthCondTypeCreateCommandTests()
        {
            _repository = Substitute.For<IHealthCondTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _handler = new HealthCondTypeCreateCommandHandler(_repository, _unitOfWork);

            _repository.AddAsync(Arg.Any<Result<HealthCondType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCondType>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<HealthCondType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCondType>>());
        }

        [Theory]
        [MemberData(nameof(InvalidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Title, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new HealthCondTypeCreateCommand(Title), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidTitleInput(string Title)
        {
            //Act
            var result = await _handler.Handle(new HealthCondTypeCreateCommand(Title), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }
    }
}