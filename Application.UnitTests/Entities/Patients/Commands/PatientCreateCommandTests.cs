using Application.Entity.Patients.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Patients.Commands
{
    public class PatientCreateCommandTests : TestsTheoryData
    {
        private readonly PatientCreateCommandHandler _handler;
        private readonly IPatientRepository _repository;
        private readonly IPatientCardRepository _patientCardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PatientCreateCommandTests()
        {
            _repository = Substitute.For<IPatientRepository>();
            _patientCardRepository = Substitute.For<IPatientCardRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _handler = new PatientCreateCommandHandler(_repository, _unitOfWork, _patientCardRepository);

            _repository.AddAsync(Arg.Any<Result<Patient>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Patient>>());

            _patientCardRepository.AddAsync(Arg.Any<Result<PatientCard>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<PatientCard>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<PatientCard>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<PatientCard>>());
        }

        [Theory]
        [MemberData(nameof(InvalidNameCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new PatientCreateCommand(Name), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidNameCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new PatientCreateCommand(Name), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }
    }
}