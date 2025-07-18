using Application.Entity.HealthConds.Queries;
using Application.Entity.HealthConds.Queries.Get;
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
    public class HealthCondGetQueryTests : TestsTheoryData
    {
        private readonly HealthCondGetQueryHandler _handler;
        private readonly IHealthCondRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly HealthCondType _healthCondType;
        private readonly HealthCond _healthcond;
        private readonly HealthCondFilter _filter;

        public HealthCondGetQueryTests()
        {
            _repository = Substitute.For<IHealthCondRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _healthCondType = HealthCondType.Create(Title.Create("Title1")).Value;
            _healthcond = HealthCond.Create(_patientCard, _healthCondType).Value;
            _filter = new HealthCondFilter();
            _handler = new HealthCondGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<HealthCond, bool>>>(expr =>
                expr.Compile()(_healthcond) == true), Arg.Any<CancellationToken>())
                .Returns(_healthcond);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<HealthCond, bool>>>(expr =>
                expr.Compile()(_healthcond) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<HealthCond>(PersistenceErrors.Entity<HealthCond>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new HealthCondGetQuery(HealthCondQueries.GetById(_healthcond.Id)), default);

            //Assert
            result.Value.TypeId.Should().Be(_healthCondType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new HealthCondGetQuery(HealthCondQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<HealthCond>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new HealthCondGetQuery(HealthCondQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.TypeId.Should().Be(_healthCondType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new HealthCondGetQuery(HealthCondQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<HealthCond>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new HealthCondGetQuery(HealthCondQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.TypeId.Should().Be(_healthCondType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new HealthCondGetQuery(HealthCondQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<HealthCond>.NotFound);
        }
    }
}