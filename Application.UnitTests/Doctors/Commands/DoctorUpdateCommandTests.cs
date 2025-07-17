using Application.Entity.Doctors.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Doctors.Commands
{
    public class DoctorUpdateCommandTests : TestsTheoryData
    {
        private readonly DoctorUpdateCommandHandler _handler;
        private readonly IDoctorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Doctor doctor = Doctor.Create(Username.Create("Fullname").Value).Value;

        public DoctorUpdateCommandTests()
        {
            _repository = Substitute.For<IDoctorRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _handler = new DoctorUpdateCommandHandler(_repository, _unitOfWork);

            _repository.UpdateAsync(Arg.Any<Result<Doctor>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Doctor>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == doctor.Id), Arg.Any<CancellationToken>())
                .Returns(doctor);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != doctor.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<Doctor>(PersistenceErrors.Entity<Doctor>.IsSoftDeleted));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<Doctor>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Doctor>>());
        }

        [Theory]
        [MemberData(nameof(InvalidNameCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new DoctorUpdateCommand(doctor.Id, Name), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidNameCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new DoctorUpdateCommand(doctor.Id, Name), default);

            //Assert
            result.Value.Should().Be(doctor.Id);
            doctor.Fullname.Should().Be(Username.Create(Name).Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new DoctorUpdateCommand(Guid.Parse(id), doctor.Fullname.Value), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<Doctor>.IsSoftDeleted);
        }
    }
}