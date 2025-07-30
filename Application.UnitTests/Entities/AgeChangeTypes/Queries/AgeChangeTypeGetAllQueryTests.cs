using Application.Entity.AgeChangeTypes.Queries;
using Application.Entity.AgeChangeTypes.Queries.GetAll;
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
    public class AgeChangeTypeGetAllQueryTests : TestsTheoryData
    {
        private readonly AgeChangeTypeGetAllQueryHandler _handler;
        private readonly IAgeChangeTypeRepository _repository;
        private readonly AgeChangeType _agechangetype1;
        private readonly AgeChangeType _agechangetype2;
        private readonly AgeChangeTypeFilter _filter;

        public AgeChangeTypeGetAllQueryTests()
        {
            _repository = Substitute.For<IAgeChangeTypeRepository>();
            _agechangetype1 = AgeChangeType.Create(Title.Create("Fullname1").Value).Value;
            _agechangetype2 = AgeChangeType.Create(Title.Create("Fullname2").Value).Value;
            _filter = new AgeChangeTypeFilter();
            _handler = new AgeChangeTypeGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChangeType, bool>>>(expr =>
                expr.Compile()(_agechangetype1) == true && expr.Compile()(_agechangetype2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChangeType>() { _agechangetype1, _agechangetype2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChangeType, bool>>>(expr =>
                expr.Compile()(_agechangetype1) == true && expr.Compile()(_agechangetype2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChangeType>() { _agechangetype1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChangeType, bool>>>(expr =>
                expr.Compile()(_agechangetype2) == true && expr.Compile()(_agechangetype1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChangeType>() { _agechangetype2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChangeType, bool>>>(expr =>
                expr.Compile()(_agechangetype1) == false && expr.Compile()(_agechangetype2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChangeType>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].Id.Should().Be(_agechangetype1.Id);
            result.Value[1].Id.Should().Be(_agechangetype2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_agechangetype1.Id);
            result.Value[1].Id.Should().Be(_agechangetype2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_agechangetype1.Id);
            result.Value[1].Id.Should().Be(_agechangetype2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<AgeChangeType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChangeType>() { _agechangetype1, _agechangetype2 }.Skip(startIndex).Take(count).ToList());

            //Act
            var result = await _handler.Handle(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<AgeChangeType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<AgeChangeType>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<AgeChangeType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<AgeChangeType>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<AgeChangeType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<AgeChangeType>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}