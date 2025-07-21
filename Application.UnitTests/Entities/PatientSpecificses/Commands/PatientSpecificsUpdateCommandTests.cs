using Application.Entity.PatientSpecificses.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.PatientSpecificss.Commands
{
    public class PatientSpecificsUpdateCommandTests : TestsTheoryData
    {
        private readonly PatientSpecificsUpdateCommandHandler _handler;
        private readonly IPatientSpecificsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PatientSpecifics _patientspecifics;

        public PatientSpecificsUpdateCommandTests()
        {
            _repository = Substitute.For<IPatientSpecificsRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patientspecifics = PatientSpecifics.Create("", "", "", "", Patient.Create(Username.Create("Fullname")).Value.Card).Value;

            _handler = new PatientSpecificsUpdateCommandHandler(_repository, _unitOfWork);
            _repository.UpdateAsync(Arg.Any<Result<PatientSpecifics>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<PatientSpecifics>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _patientspecifics.Id), Arg.Any<CancellationToken>())
                .Returns(_patientspecifics);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _patientspecifics.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<PatientSpecifics>(PersistenceErrors.Entity<PatientSpecifics>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<PatientSpecifics>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<PatientSpecifics>>());
        }

        [Theory]
        [MemberData(nameof(ValidPatientSpecificsesUpdateTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidInput(string Sleep, string Diet, string Sport, string WorkEnv)
        {
            //Act
            var result = await _handler.Handle(new PatientSpecificsUpdateCommand(_patientspecifics.Id, Sleep, Diet, Sport, WorkEnv), default);

            //Assert
            result.Value.Should().Be(_patientspecifics.Id);
        }
    }
}