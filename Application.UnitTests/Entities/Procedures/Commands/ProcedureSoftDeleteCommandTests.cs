using Application.Entity.Procedures.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Procedures.Commands
{
    public class ProcedureSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly ProcedureSoftDeleteCommandHandler _handler;
        private readonly IProcedureRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly ProcedureType _procedureType;
        private readonly Procedure _procedure;

        public ProcedureSoftDeleteCommandTests()
        {
            _repository = Substitute.For<IProcedureRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _procedureType = ProcedureType.Create(Title.Create("Title1"), "", 0).Value;
            _procedure = Procedure.Create(_patientCard, _procedureType, _procedureType.StandartDuration).Value;
            _handler = new ProcedureSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<Procedure>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Procedure>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _procedure.Id), Arg.Any<CancellationToken>(), FetchMode.Include)
                .Returns(_procedure);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _procedure.Id), Arg.Any<CancellationToken>(), FetchMode.Include)
                .Returns(Result.Failure<Procedure>(PersistenceErrors.Entity<Procedure>.IsSoftDeleted));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<Procedure>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Procedure>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ProcedureSoftDeleteCommand(_procedure.Id), default);

            //Assert
            result.Value.Should().Be(_procedure.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new ProcedureSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<Procedure>.IsSoftDeleted);
        }
    }
}