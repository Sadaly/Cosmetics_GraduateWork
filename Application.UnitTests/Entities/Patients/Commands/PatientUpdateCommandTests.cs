using Application.Entity.Patients.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Patients.Commands
{
    public class PatientUpdateCommandTests : TestsTheoryData
    {
        private readonly PatientUpdateCommandHandler _handler;
        private readonly IPatientRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;

        public PatientUpdateCommandTests()
        {
            _repository = Substitute.For<IPatientRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname").Value).Value;

            _handler = new PatientUpdateCommandHandler(_repository, _unitOfWork);
            _repository.UpdateAsync(Arg.Any<Result<Patient>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Patient>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _patient.Id), Arg.Any<CancellationToken>())
                .Returns(_patient);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _patient.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<Patient>(PersistenceErrors.Entity<Patient>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<Patient>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Patient>>());
        }

        [Theory]
        [MemberData(nameof(InvalidNameCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new PatientUpdateCommand(_patient.Id, Name), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidNameCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new PatientUpdateCommand(_patient.Id, Name), default);

            //Assert
            result.Value.Should().Be(_patient.Id);
            _patient.Fullname.Should().Be(Username.Create(Name).Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new PatientUpdateCommand(Guid.Parse(id), _patient.Fullname.Value), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<Patient>.NotFound);
        }
    }
}