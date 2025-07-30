using Application.Entity.ReservedDates.Queries;
using Application.Entity.ReservedDates.Queries.GetAll;
using Application.UnitTests.TheoryData;
using Domain.Entity;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.SupportData.Filters;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.ReservedDates.Queries
{
    public class ReservedDateGetAllQueryTests : TestsTheoryData
    {
        private readonly ReservedDateGetAllQueryHandler _handler;
        private readonly IReservedDateRepository _repository;
        private readonly ReservedDate _reserveddate1;
        private readonly ReservedDate _reserveddate2;
        private readonly ReservedDateFilter _filter;

        public ReservedDateGetAllQueryTests()
        {
            _repository = Substitute.For<IReservedDateRepository>();
            _reserveddate1 = ReservedDate.Create(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), ReservedDateType.HolidayRestrict).Value;
            _reserveddate2 = ReservedDate.Create(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), ReservedDateType.DayOfWeekRestrict).Value;
            _filter = new ReservedDateFilter();
            _handler = new ReservedDateGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<ReservedDate, bool>>>(expr =>
                expr.Compile()(_reserveddate1) == true && expr.Compile()(_reserveddate2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<ReservedDate>() { _reserveddate1, _reserveddate2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<ReservedDate, bool>>>(expr =>
                expr.Compile()(_reserveddate1) == true && expr.Compile()(_reserveddate2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<ReservedDate>() { _reserveddate1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<ReservedDate, bool>>>(expr =>
                expr.Compile()(_reserveddate2) == true && expr.Compile()(_reserveddate1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<ReservedDate>() { _reserveddate2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<ReservedDate, bool>>>(expr =>
                expr.Compile()(_reserveddate1) == false && expr.Compile()(_reserveddate2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<ReservedDate>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ReservedDateGetAllQuery(ReservedDateQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].Type.Should().Be(_reserveddate1.Type);
            result.Value[1].Type.Should().Be(_reserveddate2.Type);
        }

        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ReservedDateGetAllQuery(ReservedDateQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Type.Should().Be(_reserveddate1.Type);
            result.Value[1].Type.Should().Be(_reserveddate2.Type);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ReservedDateGetAllQuery(ReservedDateQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<ReservedDate, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<ReservedDate>() { _reserveddate1, _reserveddate2 }.Skip(startIndex).Take(count).ToList());

            //Act
            var result = await _handler.Handle(new ReservedDateGetAllQuery(ReservedDateQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<ReservedDate, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<ReservedDate>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<ReservedDate, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<ReservedDate>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<ReservedDate, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<ReservedDate>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new ReservedDateGetAllQuery(ReservedDateQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}