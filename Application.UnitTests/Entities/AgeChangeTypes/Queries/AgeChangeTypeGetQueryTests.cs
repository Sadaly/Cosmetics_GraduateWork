using Application.Entity.AgeChangeTypes.Queries;
using Application.Entity.AgeChangeTypes.Queries.Get;
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

namespace Application.UnitTests.Entities.AgeChangeTypes.Queries
{
    public class AgeChangeTypeGetQueryTests : TestsTheoryData
    {
        private readonly AgeChangeTypeGetQueryHandler _handler;
        private readonly IAgeChangeTypeRepository _repository;
        private readonly AgeChangeType _agechangetype;
        private readonly AgeChangeTypeFilter _filter;

        public AgeChangeTypeGetQueryTests()
        {
            _repository = Substitute.For<IAgeChangeTypeRepository>();
            _agechangetype = AgeChangeType.Create(Title.Create("Fullname").Value).Value;
            _filter = new AgeChangeTypeFilter();
            _handler = new AgeChangeTypeGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<AgeChangeType, bool>>>(expr =>
                expr.Compile()(_agechangetype) == true), Arg.Any<CancellationToken>())
                .Returns(_agechangetype);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<AgeChangeType, bool>>>(expr =>
                expr.Compile()(_agechangetype) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<AgeChangeType>(PersistenceErrors.Entity<AgeChangeType>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetById(_agechangetype.Id)), default);

            //Assert
            result.Value.Id.Should().Be(_agechangetype.Id);
            result.Value.Title.Should().Be(_agechangetype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<AgeChangeType>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_agechangetype.Id);
            result.Value.Title.Should().Be(_agechangetype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<AgeChangeType>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_agechangetype.Id);
            result.Value.Title.Should().Be(_agechangetype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<AgeChangeType>.NotFound);
        }
    }
}