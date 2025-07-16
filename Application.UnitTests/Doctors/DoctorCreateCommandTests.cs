using Application.Entity.Doctors.Commands.Create;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Doctors
{
    public class DoctorCreateCommandTests
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

        public static TheoryData<string, string> InvalidNameTestCases = new()
        {
            { "", DomainErrors.Username.Empty.Code },
            { new string('a', Username.MAX_LENGTH + 1), DomainErrors.Username.TooLong.Code },
            { new string('a', Username.MIN_LENGTH - 1), DomainErrors.Username.TooShort.Code }
        };

        [Theory]
        [MemberData(nameof(InvalidNameTestCases))]
        public async Task Handle_Should_ReturnError_WhenIncorrectNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new DoctorCreateCommand(Name), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        public static TheoryData<string> ValidNameTestCases = new()
        {
            { new string('a', Username.MAX_LENGTH - Username.MIN_LENGTH)},
            { new string('a', Username.MAX_LENGTH) },
            { new string('a', Username.MIN_LENGTH) }
        };

        [Theory]
        [MemberData(nameof(ValidNameTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenCorrectNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new DoctorCreateCommand(Name), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }
    }
}