using Application.Entity.SkinCareTypes.Queries;
using Application.Entity.SkinCareTypes.Queries.GetAll;
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

namespace Application.UnitTests.Entities.SkinCareTypes.Queries
{
    public class SkinCareTypeGetAllQueryTests : TestsTheoryData
    {
        private readonly SkinCareTypeGetAllQueryHandler _handler;
        private readonly ISkinCareTypeRepository _repository;
        private readonly SkinCareType _skincaretype1;
        private readonly SkinCareType _skincaretype2;
        private readonly SkinCareTypeFilter _filter;

        public SkinCareTypeGetAllQueryTests()
        {
            _repository = Substitute.For<ISkinCareTypeRepository>();
            _skincaretype1 = SkinCareType.Create(Title.Create("Fullname1").Value).Value;
            _skincaretype2 = SkinCareType.Create(Title.Create("Fullname2").Value).Value;
            _filter = new SkinCareTypeFilter();
            _handler = new SkinCareTypeGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinCareType, bool>>>(expr =>
                expr.Compile()(_skincaretype1) == true && expr.Compile()(_skincaretype2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<SkinCareType>() { _skincaretype1, _skincaretype2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinCareType, bool>>>(expr =>
                expr.Compile()(_skincaretype1) == true && expr.Compile()(_skincaretype2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinCareType>() { _skincaretype1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinCareType, bool>>>(expr =>
                expr.Compile()(_skincaretype2) == true && expr.Compile()(_skincaretype1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinCareType>() { _skincaretype2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinCareType, bool>>>(expr =>
                expr.Compile()(_skincaretype1) == false && expr.Compile()(_skincaretype2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinCareType>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].Id.Should().Be(_skincaretype1.Id);
            result.Value[1].Id.Should().Be(_skincaretype2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_skincaretype1.Id);
            result.Value[1].Id.Should().Be(_skincaretype2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_skincaretype1.Id);
            result.Value[1].Id.Should().Be(_skincaretype2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<SkinCareType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<SkinCareType>() { _skincaretype1, _skincaretype2 }.Skip(startIndex).Take(count).ToList());

            //Act
            var result = await _handler.Handle(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<SkinCareType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<SkinCareType>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<SkinCareType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<SkinCareType>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<SkinCareType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<SkinCareType>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}