using Application.Entity.SkinFeatureTypes.Queries;
using Application.Entity.SkinFeatureTypes.Queries.GetAll;
using Application.UnitTests.TheoryData;
using Domain.Entity;
using Domain.Repositories;
using Domain.SupportData.Filters;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.SkinFeatureTypes.Queries
{
    public class SkinFeatureTypeGetAllQueryTests : TestsTheoryData
    {
        private readonly SkinFeatureTypeGetAllQueryHandler _handler;
        private readonly ISkinFeatureTypeRepository _repository;
        private readonly SkinFeatureType _skinfeaturetype1;
        private readonly SkinFeatureType _skinfeaturetype2;
        private readonly SkinFeatureTypeFilter _filter;

        public SkinFeatureTypeGetAllQueryTests()
        {
            _repository = Substitute.For<ISkinFeatureTypeRepository>();
            _skinfeaturetype1 = SkinFeatureType.Create(Title.Create("Fullname1").Value).Value;
            _skinfeaturetype2 = SkinFeatureType.Create(Title.Create("Fullname2").Value).Value;
            _filter = new SkinFeatureTypeFilter();
            _handler = new SkinFeatureTypeGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinFeatureType, bool>>>(expr =>
                expr.Compile()(_skinfeaturetype1) == true && expr.Compile()(_skinfeaturetype2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeatureType>(){ _skinfeaturetype1, _skinfeaturetype2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinFeatureType, bool>>>(expr =>
                expr.Compile()(_skinfeaturetype1) == true && expr.Compile()(_skinfeaturetype2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeatureType>(){ _skinfeaturetype1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinFeatureType, bool>>>(expr =>
                expr.Compile()(_skinfeaturetype2) == true && expr.Compile()(_skinfeaturetype1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeatureType>(){ _skinfeaturetype2 });
            
            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinFeatureType, bool>>>(expr =>
                expr.Compile()(_skinfeaturetype1) == false && expr.Compile()(_skinfeaturetype2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeatureType>(){});
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].Id.Should().Be(_skinfeaturetype1.Id);
            result.Value[1].Id.Should().Be(_skinfeaturetype2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_skinfeaturetype1.Id);
            result.Value[1].Id.Should().Be(_skinfeaturetype2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_skinfeaturetype1.Id);
            result.Value[1].Id.Should().Be(_skinfeaturetype2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<SkinFeatureType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeatureType>() { _skinfeaturetype1, _skinfeaturetype2 }.Skip(startIndex).Take(count).ToList());
            
            //Act
            var result = await _handler.Handle(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
    }
}