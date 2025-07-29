using Application.Entity.Doctors.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Doctors.Commands
{
    public class DoctorCreateCommandTests : TestsTheoryData
    {
        private readonly DoctorCreateCommandHandler _handler;
        private readonly IDoctorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorCreateCommandTests()
        {
            _repository = Substitute.For<IDoctorRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _handler = new DoctorCreateCommandHandler(_repository, _unitOfWork);

            _repository.AddAsync(Arg.Any<Result<Doctor>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Doctor>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<Doctor>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Doctor>>());
        }

        [Theory]
        [MemberData(nameof(InvalidNameCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new DoctorCreateCommand(Name), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidNameCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new DoctorCreateCommand(Name), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }
    }
}