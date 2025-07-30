using Application.Entity.AgeChanges.Queries;
using Application.Entity.AgeChanges.Queries.GetAll;
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

namespace Application.UnitTests.Entities.AgeChanges.Queries
{
    public class AgeChangeGetAllQueryTests : TestsTheoryData
    {
        private readonly AgeChangeGetAllQueryHandler _handler;
        private readonly IAgeChangeRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly AgeChangeType _ageChangeType1;
        private readonly AgeChangeType _ageChangeType2;
        private readonly AgeChange _agechange1;
        private readonly AgeChange _agechange2;
        private readonly AgeChangeFilter _filter;

        public AgeChangeGetAllQueryTests()
        {
            _repository = Substitute.For<IAgeChangeRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _ageChangeType1 = AgeChangeType.Create(Title.Create("Title1")).Value;
            _ageChangeType2 = AgeChangeType.Create(Title.Create("Title2")).Value;
            _agechange1 = AgeChange.Create(_patientCard, _ageChangeType1).Value;
            _agechange2 = AgeChange.Create(_patientCard, _ageChangeType2).Value;
            _filter = new AgeChangeFilter();
            _handler = new AgeChangeGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
                expr.Compile()(_agechange1) == true && expr.Compile()(_agechange2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChange>() { _agechange1, _agechange2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
                expr.Compile()(_agechange1) == true && expr.Compile()(_agechange2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChange>() { _agechange1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
                expr.Compile()(_agechange2) == true && expr.Compile()(_agechange1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChange>() { _agechange2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
                expr.Compile()(_agechange1) == false && expr.Compile()(_agechange2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChange>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_ageChangeType1.Id);
            result.Value[1].TypeId.Should().Be(_ageChangeType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_ageChangeType1.Id);
            result.Value[1].TypeId.Should().Be(_ageChangeType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneTitleGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneTitle(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_ageChangeType1.Id);
            result.Value[1].TypeId.Should().Be(_ageChangeType2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }
        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<AgeChange, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<AgeChange>() { _agechange1, _agechange2 }.Skip(startIndex).Take(count).ToList());

            //Act
            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<AgeChange, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<AgeChange>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<AgeChange, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<AgeChange>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<AgeChange, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<AgeChange>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}