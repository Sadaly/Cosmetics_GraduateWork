using Application.Entity.SkinCares.Queries;
using Application.Entity.SkinCares.Queries.Get;
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

namespace Application.UnitTests.Entities.SkinCares.Queries
{
    public class SkinCareGetQueryTests : TestsTheoryData
    {
        private readonly SkinCareGetQueryHandler _handler;
        private readonly ISkinCareRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly SkinCareType _skinCareType;
        private readonly SkinCare _skincare;
        private readonly SkinCareFilter _filter;

        public SkinCareGetQueryTests()
        {
            _repository = Substitute.For<ISkinCareRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _skinCareType = SkinCareType.Create(Title.Create("Title1")).Value;
            _skincare = SkinCare.Create(_patientCard, _skinCareType).Value;
            _filter = new SkinCareFilter();
            _handler = new SkinCareGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<SkinCare, bool>>>(expr =>
                expr.Compile()(_skincare) == true), Arg.Any<CancellationToken>())
                .Returns(_skincare);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<SkinCare, bool>>>(expr =>
                expr.Compile()(_skincare) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinCare>(PersistenceErrors.Entity<SkinCare>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinCareGetQuery(SkinCareQueries.GetById(_skincare.Id)), default);

            //Assert
            result.Value.TypeId.Should().Be(_skinCareType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new SkinCareGetQuery(SkinCareQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinCare>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new SkinCareGetQuery(SkinCareQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.TypeId.Should().Be(_skinCareType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new SkinCareGetQuery(SkinCareQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinCare>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinCareGetQuery(SkinCareQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.TypeId.Should().Be(_skinCareType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinCareGetQuery(SkinCareQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinCare>.NotFound);
        }
    }
}