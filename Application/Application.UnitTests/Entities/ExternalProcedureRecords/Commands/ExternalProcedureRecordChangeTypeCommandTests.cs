using Application.Entity.ExternalProcedureRecords.Commands.ChangeType;
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
    public class ExternalProcedureRecordChangeTypeCommandTests : TestsTheoryData
    {
        private readonly ExternalProcedureRecordChangeTypeCommandHandler _handler;
        private readonly IExternalProcedureRecordRepository _repository;
        private readonly IExternalProcedureRecordTypeRepository _typeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly ExternalProcedureRecordType _externalProcedureRecordType;
        private readonly ExternalProcedureRecordType _externalProcedureRecordTypeUpdated;
        private readonly ExternalProcedureRecord _externalprocedurerecord;


        public ExternalProcedureRecordChangeTypeCommandTests()
        {
            _repository = Substitute.For<IExternalProcedureRecordRepository>();
            _typeRepository = Substitute.For<IExternalProcedureRecordTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _externalProcedureRecordType = ExternalProcedureRecordType.Create(Title.Create("Title")).Value;
            _externalProcedureRecordTypeUpdated = ExternalProcedureRecordType.Create(Title.Create("Updated")).Value;
            _externalprocedurerecord = ExternalProcedureRecord.Create(_patientCard, _externalProcedureRecordType).Value;
            _handler = new ExternalProcedureRecordChangeTypeCommandHandler(_repository, _typeRepository, _unitOfWork);

            _repository.UpdateAsync(Arg.Any<Result<ExternalProcedureRecord>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecord>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _externalprocedurerecord.Id), Arg.Any<CancellationToken>())
                .Returns(_externalprocedurerecord);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _externalprocedurerecord.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ExternalProcedureRecord>(PersistenceErrors.Entity<ExternalProcedureRecord>.NotFound));

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _externalProcedureRecordTypeUpdated.Id), Arg.Any<CancellationToken>())
                .Returns(_externalProcedureRecordTypeUpdated);

            _typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != _externalProcedureRecordTypeUpdated.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ExternalProcedureRecordType>(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ExternalProcedureRecord>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecord>>());
        }

        [Theory]
        [MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
        public async Task Handle_Should_ReturnFailure_WhenInvalidGuids(string ExternalProcedureRecordId, string newTypeId)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordChangeTypeCommand(Guid.Parse(ExternalProcedureRecordId), Guid.Parse(newTypeId)), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidGuids()
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordChangeTypeCommand(_externalprocedurerecord.Id, _externalProcedureRecordTypeUpdated.Id), default);

            //Assert
            result.Value.Should().Be(_externalprocedurerecord.Id);
        }
    }
}