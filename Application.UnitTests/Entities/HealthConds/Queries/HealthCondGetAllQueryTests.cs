using Application.Entity.HealthConds.Queries;
using Application.Entity.HealthConds.Queries.GetAll;
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

namespace Application.UnitTests.Entities.HealthConds.Queries
{
    public class HealthCondGetAllQueryTests : TestsTheoryData
    {
        private readonly HealthCondGetAllQueryHandler _handler;
        private readonly IHealthCondRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly HealthCondType _healthCondType1;
        private readonly HealthCondType _healthCondType2;
        private readonly HealthCond _healthcond1;
        private readonly HealthCond _healthcond2;
        private readonly HealthCondFilter _filter;

        public HealthCondGetAllQueryTests()
        {
            _repository = Substitute.For<IHealthCondRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _healthCondType1 = HealthCondType.Create(Title.Create("Title1")).Value;
            _healthCondType2 = HealthCondType.Create(Title.Create("Title2")).Value;
            _healthcond1 = HealthCond.Create(_patientCard, _healthCondType1).Value;
            _healthcond2 = HealthCond.Create(_patientCard, _healthCondType2).Value;
            _filter = new HealthCondFilter();
            _handler = new HealthCondGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<HealthCond, bool>>>(expr =>
                expr.Compile()(_healthcond1) == true && expr.Compile()(_healthcond2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCond>() { _healthcond1, _healthcond2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<HealthCond, bool>>>(expr =>
                expr.Compile()(_healthcond1) == true && expr.Compile()(_healthcond2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCond>() { _healthcond1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<HealthCond, bool>>>(expr =>
                expr.Compile()(_healthcond2) == true && expr.Compile()(_healthcond1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCond>() { _healthcond2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<HealthCond, bool>>>(expr =>
                expr.Compile()(_healthcond1) == false && expr.Compile()(_healthcond2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<HealthCond>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new HealthCondGetAllQuery(HealthCondQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_healthCondType1.Id);
            result.Value[1].TypeId.Should().Be(_healthCondType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_healthCondType1.Id);
            result.Value[1].TypeId.Should().Be(_healthCondType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneTitleGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneTitle(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_healthCondType1.Id);
            result.Value[1].TypeId.Should().Be(_healthCondType2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<HealthCond, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<HealthCond>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<HealthCond, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<HealthCond>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<HealthCond, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<HealthCond>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new HealthCondGetAllQuery(HealthCondQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}