using Application.Entity.SkinFeatureTypes.Queries;
using Application.Entity.SkinFeatureTypes.Queries.Get;
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

namespace Application.UnitTests.Entities.SkinFeatureTypes.Queries
{
    public class SkinFeatureTypeGetQueryTests : TestsTheoryData
    {
        private readonly SkinFeatureTypeGetQueryHandler _handler;
        private readonly ISkinFeatureTypeRepository _repository;
        private readonly SkinFeatureType _skinfeaturetype;
        private readonly SkinFeatureTypeFilter _filter;

        public SkinFeatureTypeGetQueryTests()
        {
            _repository = Substitute.For<ISkinFeatureTypeRepository>();
            _skinfeaturetype = SkinFeatureType.Create(Title.Create("Fullname").Value).Value;
            _filter = new SkinFeatureTypeFilter();
            _handler = new SkinFeatureTypeGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<SkinFeatureType, bool>>>(expr =>
                expr.Compile()(_skinfeaturetype) == true), Arg.Any<CancellationToken>())
                .Returns(_skinfeaturetype);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<SkinFeatureType, bool>>>(expr =>
                expr.Compile()(_skinfeaturetype) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinFeatureType>(PersistenceErrors.Entity<SkinFeatureType>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureTypeGetQuery(SkinFeatureTypeQueries.GetById(_skinfeaturetype.Id)), default);

            //Assert
            result.Value.Id.Should().Be(_skinfeaturetype.Id);
            result.Value.Title.Should().Be(_skinfeaturetype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureTypeGetQuery(SkinFeatureTypeQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinFeatureType>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinFeatureTypeGetQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_skinfeaturetype.Id);
            result.Value.Title.Should().Be(_skinfeaturetype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinFeatureTypeGetQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinFeatureType>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinFeatureTypeGetQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_skinfeaturetype.Id);
            result.Value.Title.Should().Be(_skinfeaturetype.Title.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinFeatureTypeGetQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinFeatureType>.NotFound);
        }
    }
}