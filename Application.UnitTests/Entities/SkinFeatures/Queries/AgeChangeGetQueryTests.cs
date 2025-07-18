using Application.Entity.SkinFeatures.Queries;
using Application.Entity.SkinFeatures.Queries.Get;
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

namespace Application.UnitTests.Entities.SkinFeatures.Queries
{
    public class SkinFeatureGetQueryTests : TestsTheoryData
    {
        private readonly SkinFeatureGetQueryHandler _handler;
        private readonly ISkinFeatureRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly SkinFeatureType _skinFeatureType;
        private readonly SkinFeature _skinfeature;
        private readonly SkinFeatureFilter _filter;

        public SkinFeatureGetQueryTests()
        {
            _repository = Substitute.For<ISkinFeatureRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _skinFeatureType = SkinFeatureType.Create(Title.Create("Title1")).Value;
            _skinfeature = SkinFeature.Create(_patientCard, _skinFeatureType).Value;
            _filter = new SkinFeatureFilter();
            _handler = new SkinFeatureGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<SkinFeature, bool>>>(expr =>
                expr.Compile()(_skinfeature) == true), Arg.Any<CancellationToken>())
                .Returns(_skinfeature);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<SkinFeature, bool>>>(expr =>
                expr.Compile()(_skinfeature) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<SkinFeature>(PersistenceErrors.Entity<SkinFeature>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureGetQuery(SkinFeatureQueries.GetById(_skinfeature.Id)), default);

            //Assert
            result.Value.TypeId.Should().Be(_skinFeatureType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureGetQuery(SkinFeatureQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinFeature>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new SkinFeatureGetQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.TypeId.Should().Be(_skinFeatureType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new SkinFeatureGetQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinFeature>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinFeatureGetQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.TypeId.Should().Be(_skinFeatureType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinFeatureGetQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<SkinFeature>.NotFound);
        }
    }
}