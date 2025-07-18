using Application.Entity.ProcedureTypes.Queries;
using Application.Entity.ProcedureTypes.Queries.Get;
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

namespace Application.UnitTests.Entities.ProcedureTypes.Queries
{
    public class ProcedureTypeGetQueryTests : TestsTheoryData
    {
        private readonly ProcedureTypeGetQueryHandler _handler;
        private readonly IProcedureTypeRepository _repository;
        private readonly ProcedureType _proceduretype;
        private readonly ProcedureTypeFilter _filter;

        public ProcedureTypeGetQueryTests()
        {
            _repository = Substitute.For<IProcedureTypeRepository>();
            _proceduretype = ProcedureType.Create(Title.Create("Fullname").Value, "", 0).Value;
            _filter = new ProcedureTypeFilter();
            _handler = new ProcedureTypeGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<ProcedureType, bool>>>(expr =>
                expr.Compile()(_proceduretype) == true), Arg.Any<CancellationToken>())
                .Returns(_proceduretype);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<ProcedureType, bool>>>(expr =>
                expr.Compile()(_proceduretype) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ProcedureType>(PersistenceErrors.Entity<ProcedureType>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ProcedureTypeGetQuery(ProcedureTypeQueries.GetById(_proceduretype.Id)), default);

            //Assert
            result.Value.Id.Should().Be(_proceduretype.Id);
            result.Value.Title.Should().Be(_proceduretype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new ProcedureTypeGetQuery(ProcedureTypeQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<ProcedureType>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new ProcedureTypeGetQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_proceduretype.Id);
            result.Value.Title.Should().Be(_proceduretype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new ProcedureTypeGetQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<ProcedureType>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ProcedureTypeGetQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_proceduretype.Id);
            result.Value.Title.Should().Be(_proceduretype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ProcedureTypeGetQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<ProcedureType>.NotFound);
        }
    }
}