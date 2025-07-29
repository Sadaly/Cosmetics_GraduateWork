using Application.Entity.HealthCondTypes.Queries;
using Application.Entity.HealthCondTypes.Queries.GetAll;
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

namespace Application.UnitTests.Entities.HealthCondTypes.Queries
{
    public class HealthCondTypeGetAllQueryTests : TestsTheoryData
    {
        private readonly HealthCondTypeGetAllQueryHandler _handler;
        private readonly IHealthCondTypeRepository _repository;
        private readonly HealthCondType _healthcondtype1;
        private readonly HealthCondType _healthcondtype2;
        private readonly HealthCondTypeFilter _filter;

        public HealthCondTypeGetAllQueryTests()
        {
            _repository = Substitute.For<IHealthCondTypeRepository>();
            _healthcondtype1 = HealthCondType.Create(Title.Create("Fullname1").Value).Value;
            _healthcondtype2 = HealthCondType.Create(Title.Create("Fullname2").Value).Value;
            _filter = new HealthCondTypeFilter();
            _handler = new HealthCondTypeGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<HealthCondType, bool>>>(expr =>
                expr.Compile()(_healthcondtype1) == true && expr.Compile()(_healthcondtype2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCondType>(){ _healthcondtype1, _healthcondtype2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<HealthCondType, bool>>>(expr =>
                expr.Compile()(_healthcondtype1) == true && expr.Compile()(_healthcondtype2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCondType>(){ _healthcondtype1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<HealthCondType, bool>>>(expr =>
                expr.Compile()(_healthcondtype2) == true && expr.Compile()(_healthcondtype1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCondType>(){ _healthcondtype2 });
            
            _repository.GetAllAsync(Arg.Is<Expression<Func<HealthCondType, bool>>>(expr =>
                expr.Compile()(_healthcondtype1) == false && expr.Compile()(_healthcondtype2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCondType>(){});
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].Id.Should().Be(_healthcondtype1.Id);
            result.Value[1].Id.Should().Be(_healthcondtype2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_healthcondtype1.Id);
            result.Value[1].Id.Should().Be(_healthcondtype2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_healthcondtype1.Id);
            result.Value[1].Id.Should().Be(_healthcondtype2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<HealthCondType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCondType>() { _healthcondtype1, _healthcondtype2 }.Skip(startIndex).Take(count).ToList());
            
            //Act
            var result = await _handler.Handle(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<HealthCondType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<HealthCondType>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<HealthCondType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<HealthCondType>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<HealthCondType, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<HealthCondType>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}