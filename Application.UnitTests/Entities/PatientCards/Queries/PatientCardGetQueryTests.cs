using Application.Entity.PatientCards.Queries;
using Application.Entity.PatientCards.Queries.Get;
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

namespace Application.UnitTests.Entities.PatientCards.Queries
{
    public class PatientCardGetQueryTests : TestsTheoryData
    {
        private readonly PatientCardGetQueryHandler _handler;
        private readonly IPatientCardRepository _repository;
        private readonly PatientCard _patientcard;
        private readonly PatientCardFilter _filter;

        public PatientCardGetQueryTests()
        {
            _repository = Substitute.For<IPatientCardRepository>();
            _patientcard = Patient.Create(Username.Create("Fullname")).Value.Card;
            _filter = new PatientCardFilter();
            _handler = new PatientCardGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<PatientCard, bool>>>(expr =>
                expr.Compile()(_patientcard) == true), Arg.Any<CancellationToken>())
                .Returns(_patientcard);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<PatientCard, bool>>>(expr =>
                expr.Compile()(_patientcard) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<PatientCard>(PersistenceErrors.Entity<PatientCard>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new PatientCardGetQuery(PatientCardQueries.GetById(_patientcard.Id)), default);

            //Assert
            result.Value.Id.Should().Be(_patientcard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new PatientCardGetQuery(PatientCardQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<PatientCard>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new PatientCardGetQuery(PatientCardQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_patientcard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new PatientCardGetQuery(PatientCardQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<PatientCard>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new PatientCardGetQuery(PatientCardQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Id.Should().Be(_patientcard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new PatientCardGetQuery(PatientCardQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<PatientCard>.NotFound);
        }
    }
}