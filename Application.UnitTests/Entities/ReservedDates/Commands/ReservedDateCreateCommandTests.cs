using Application.Entity.ReservedDates.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ReservedDates.Commands
{
    public class ReservedDateCreateCommandTests : TestsTheoryData
    {
        private readonly ReservedDateCreateCommandHandler _handler;
        private readonly IReservedDateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ReservedDateCreateCommandTests()
        {
            _repository = Substitute.For<IReservedDateRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _handler = new ReservedDateCreateCommandHandler(_repository, _unitOfWork);

            _repository.AddAsync(Arg.Any<Result<ReservedDate>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ReservedDate>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ReservedDate>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ReservedDate>>());
        }

        [Theory]
        [MemberData(nameof(ValidReservedDateTypeCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidInput(Domain.Enums.ReservedDateType ReservedDateType)
        {
            //Act
            var result = await _handler.Handle(new ReservedDateCreateCommand(
                DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), ReservedDateType), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }
    }
}