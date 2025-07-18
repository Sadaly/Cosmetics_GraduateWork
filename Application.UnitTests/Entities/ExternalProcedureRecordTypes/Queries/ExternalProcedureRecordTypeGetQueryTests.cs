using Application.Entity.ExternalProcedureRecordTypes.Queries;
using Application.Entity.ExternalProcedureRecordTypes.Queries.Get;
using Application.UnitTests.TheoryData;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.SupportData.Filters;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.ExternalProcedureRecordTypes.Queries
{
    public class ExternalProcedureRecordTypeGetQueryTests : TestsTheoryData
    {
        private readonly ExternalProcedureRecordTypeGetQueryHandler _handler;
        private readonly IExternalProcedureRecordTypeRepository _repository;
        private readonly ExternalProcedureRecordType _externalprocedurerecordtype;
        private readonly ExternalProcedureRecordTypeFilter _filter;

        public ExternalProcedureRecordTypeGetQueryTests()
        {
            _repository = Substitute.For<IExternalProcedureRecordTypeRepository>();
            _externalprocedurerecordtype = ExternalProcedureRecordType.Create(Title.Create("Fullname").Value).Value;
            _filter = new ExternalProcedureRecordTypeFilter();
            _handler = new ExternalProcedureRecordTypeGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<ExternalProcedureRecordType, bool>>>(expr =>
                expr.Compile()(_externalprocedurerecordtype) == true), Arg.Any<CancellationToken>())
                .Returns(_externalprocedurerecordtype);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<ExternalProcedureRecordType, bool>>>(expr =>
                expr.Compile()(_externalprocedurerecordtype) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ExternalProcedureRecordType>(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordTypeGetQuery(ExternalProcedureRecordTypeQueries.GetById(_externalprocedurerecordtype.Id)), default);

            //Assert
            result.Value.Id.Should().Be(_externalprocedurerecordtype.Id);
            result.Value.Title.Should().Be(_externalprocedurerecordtype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordTypeGetQuery(ExternalProcedureRecordTypeQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new ExternalProcedureRecordTypeGetQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_externalprocedurerecordtype.Id);
            result.Value.Title.Should().Be(_externalprocedurerecordtype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new ExternalProcedureRecordTypeGetQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ExternalProcedureRecordTypeGetQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_externalprocedurerecordtype.Id);
            result.Value.Title.Should().Be(_externalprocedurerecordtype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ExternalProcedureRecordTypeGetQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound);
        }
    }
}