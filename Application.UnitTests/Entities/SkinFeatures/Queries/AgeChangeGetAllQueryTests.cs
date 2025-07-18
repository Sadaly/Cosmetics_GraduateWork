using Application.Entity.SkinFeatures.Queries;
using Application.Entity.SkinFeatures.Queries.GetAll;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.SupportData.Filters;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.SkinFeatures.Queries
{
    public class SkinFeatureGetAllQueryTests : TestsTheoryData
    {
        private readonly SkinFeatureGetAllQueryHandler _handler;
        private readonly ISkinFeatureRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly SkinFeatureType _skinFeatureType1;
        private readonly SkinFeatureType _skinFeatureType2;
        private readonly SkinFeature _skinfeature1;
        private readonly SkinFeature _skinfeature2;
        private readonly SkinFeatureFilter _filter;

        public SkinFeatureGetAllQueryTests()
        {
            _repository = Substitute.For<ISkinFeatureRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _skinFeatureType1 = SkinFeatureType.Create(Title.Create("Title1")).Value;
            _skinFeatureType2 = SkinFeatureType.Create(Title.Create("Title2")).Value;
            _skinfeature1 = SkinFeature.Create(_patientCard, _skinFeatureType1).Value;
            _skinfeature2 = SkinFeature.Create(_patientCard, _skinFeatureType2).Value;
            _filter = new SkinFeatureFilter();
            _handler = new SkinFeatureGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinFeature, bool>>>(expr =>
                expr.Compile()(_skinfeature1) == true && expr.Compile()(_skinfeature2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeature>() { _skinfeature1, _skinfeature2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinFeature, bool>>>(expr =>
                expr.Compile()(_skinfeature1) == true && expr.Compile()(_skinfeature2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeature>() { _skinfeature1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinFeature, bool>>>(expr =>
                expr.Compile()(_skinfeature2) == true && expr.Compile()(_skinfeature1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeature>() { _skinfeature2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<SkinFeature, bool>>>(expr =>
                expr.Compile()(_skinfeature1) == false && expr.Compile()(_skinfeature2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<SkinFeature>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new SkinFeatureGetAllQuery(SkinFeatureQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_skinFeatureType1.Id);
            result.Value[1].TypeId.Should().Be(_skinFeatureType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new SkinFeatureGetAllQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_skinFeatureType1.Id);
            result.Value[1].TypeId.Should().Be(_skinFeatureType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneTitleGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneTitle(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new SkinFeatureGetAllQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new SkinFeatureGetAllQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new SkinFeatureGetAllQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_skinFeatureType1.Id);
            result.Value[1].TypeId.Should().Be(_skinFeatureType2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new SkinFeatureGetAllQuery(SkinFeatureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }
    }
}