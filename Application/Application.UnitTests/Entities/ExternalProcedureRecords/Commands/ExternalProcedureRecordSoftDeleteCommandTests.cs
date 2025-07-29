using Application.Entity.ExternalProcedureRecords.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ExternalProcedureRecords.Commands
{
    public class ExternalProcedureRecordSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly ExternalProcedureRecordSoftDeleteCommandHandler _handler;
        private readonly IExternalProcedureRecordRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly ExternalProcedureRecordType _externalProcedureRecordType;
        private readonly ExternalProcedureRecord _externalprocedurerecord;

        public ExternalProcedureRecordSoftDeleteCommandTests()
        {
            _repository = Substitute.For<IExternalProcedureRecordRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _externalProcedureRecordType = ExternalProcedureRecordType.Create(Title.Create("Title")).Value;
            _externalprocedurerecord = ExternalProcedureRecord.Create(_patientCard, _externalProcedureRecordType).Value;
            _handler = new ExternalProcedureRecordSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<ExternalProcedureRecord>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecord>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _externalprocedurerecord.Id), Arg.Any<CancellationToken>())
                .Returns(_externalprocedurerecord);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _externalprocedurerecord.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ExternalProcedureRecord>(PersistenceErrors.Entity<ExternalProcedureRecord>.IsSoftDeleted));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ExternalProcedureRecord>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecord>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordSoftDeleteCommand(_externalprocedurerecord.Id), default);

            //Assert
            result.Value.Should().Be(_externalprocedurerecord.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecord>.IsSoftDeleted);
        }
    }
}