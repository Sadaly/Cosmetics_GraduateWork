using Application.Entity.ExternalProcedureRecords.Queries;
using Application.Entity.ExternalProcedureRecords.Queries.GetAll;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.SupportData.Filters;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.ExternalProcedureRecords.Queries
{
    public class ExternalProcedureRecordGetAllQueryTests : TestsTheoryData
    {
        private readonly ExternalProcedureRecordGetAllQueryHandler _handler;
        private readonly IExternalProcedureRecordRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly ExternalProcedureRecordType _externalProcedureRecordType1;
        private readonly ExternalProcedureRecordType _externalProcedureRecordType2;
        private readonly ExternalProcedureRecord _externalprocedurerecord1;
        private readonly ExternalProcedureRecord _externalprocedurerecord2;
        private readonly ExternalProcedureRecordFilter _filter;

        public ExternalProcedureRecordGetAllQueryTests()
        {
            _repository = Substitute.For<IExternalProcedureRecordRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _externalProcedureRecordType1 = ExternalProcedureRecordType.Create(Title.Create("Title1")).Value;
            _externalProcedureRecordType2 = ExternalProcedureRecordType.Create(Title.Create("Title2")).Value;
            _externalprocedurerecord1 = ExternalProcedureRecord.Create(_patientCard, _externalProcedureRecordType1).Value;
            _externalprocedurerecord2 = ExternalProcedureRecord.Create(_patientCard, _externalProcedureRecordType2).Value;
            _filter = new ExternalProcedureRecordFilter();
            _handler = new ExternalProcedureRecordGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<ExternalProcedureRecord, bool>>>(expr =>
                expr.Compile()(_externalprocedurerecord1) == true && expr.Compile()(_externalprocedurerecord2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<ExternalProcedureRecord>() { _externalprocedurerecord1, _externalprocedurerecord2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<ExternalProcedureRecord, bool>>>(expr =>
                expr.Compile()(_externalprocedurerecord1) == true && expr.Compile()(_externalprocedurerecord2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<ExternalProcedureRecord>() { _externalprocedurerecord1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<ExternalProcedureRecord, bool>>>(expr =>
                expr.Compile()(_externalprocedurerecord2) == true && expr.Compile()(_externalprocedurerecord1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<ExternalProcedureRecord>() { _externalprocedurerecord2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<ExternalProcedureRecord, bool>>>(expr =>
                expr.Compile()(_externalprocedurerecord1) == false && expr.Compile()(_externalprocedurerecord2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<ExternalProcedureRecord>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordGetAllQuery(ExternalProcedureRecordQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_externalProcedureRecordType1.Id);
            result.Value[1].TypeId.Should().Be(_externalProcedureRecordType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new ExternalProcedureRecordGetAllQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_externalProcedureRecordType1.Id);
            result.Value[1].TypeId.Should().Be(_externalProcedureRecordType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneTitleGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneTitle(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new ExternalProcedureRecordGetAllQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new ExternalProcedureRecordGetAllQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ExternalProcedureRecordGetAllQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_externalProcedureRecordType1.Id);
            result.Value[1].TypeId.Should().Be(_externalProcedureRecordType2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ExternalProcedureRecordGetAllQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }
    }
}