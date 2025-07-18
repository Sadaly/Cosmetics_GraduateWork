using Application.Entity.Patients.Commands.SoftDelete;
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
    public class PatientSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly PatientSoftDeleteCommandHandler _handler;
        private readonly IPatientRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;

        public PatientSoftDeleteCommandTests()
        {
            _repository = Substitute.For<IPatientRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname").Value).Value;

            _handler = new PatientSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<Patient>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Patient>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _patient.Id), Arg.Any<CancellationToken>(), FetchMode.Include)
                .Returns(_patient);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _patient.Id), Arg.Any<CancellationToken>(), FetchMode.Include)
                .Returns(Result.Failure<Patient>(PersistenceErrors.Entity<Patient>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<Patient>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Patient>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new PatientSoftDeleteCommand(_patient.Id), default);

            //Assert
            result.Value.Should().Be(_patient.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new PatientSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<Patient>.NotFound);
        }
    }
}