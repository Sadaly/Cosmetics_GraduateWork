//using Application.Entity.AgeChanges.Queries;
//using Application.Entity.AgeChanges.Queries.Get;
//using Application.UnitTests.TheoryData;
//using Domain.Entity;
//using Domain.Errors;
//using Domain.Repositories;
//using Domain.Shared;
//using Domain.SupportData.Filters;
//using Domain.ValueObjects;
//using FluentAssertions;
//using NSubstitute;
//using System.Linq.Expressions;

//namespace Application.UnitTests.AgeChanges.Queries
//{
//    public class AgeChangeGetQueryTests : TestsTheoryData
//    {
//        private readonly AgeChangeGetQueryHandler _handler;
//        private readonly IAgeChangeRepository _repository;
//        private readonly AgeChange agechange = AgeChange.Create(Username.Create("Fullname").Value).Value;
//        private readonly AgeChangeFilter filter = new AgeChangeFilter();

//        public AgeChangeGetQueryTests()
//        {
//            _repository = Substitute.For<IAgeChangeRepository>();

//            _handler = new AgeChangeGetQueryHandler(_repository);

//            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
//                expr.Compile()(agechange) == true), Arg.Any<CancellationToken>())
//                .Returns(agechange);

//            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
//                expr.Compile()(agechange) == false), Arg.Any<CancellationToken>())
//                .Returns(Result.Failure<AgeChange>(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted));
//        }

//        [Fact]
//        public async Task Handle_Should_ReturnSuccess_WhenValidId()
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetById(agechange.Id)), default);

//            //Assert
//            result.Value.Id.Should().Be(agechange.Id);
//            result.Value.Name.Should().Be(agechange.Fullname.Value);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
//        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetById(Guid.Parse(id))), default);

//            //Assert
//            result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted);
//        }

//        [Theory]
//        [MemberData(nameof(ValidNameGetTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
//        {
//            //Act
//            filter.Name = name;
//            var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Value.Id.Should().Be(agechange.Id);
//            result.Value.Name.Should().Be(agechange.Fullname.Value);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidNameGetTestCases))]
//        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
//        {
//            //Act
//            filter.Name = name;
//            var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted);
//        }
//        [Theory]
//        [MemberData(nameof(ValidCreationDatesGetTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
//        {
//            //Act
//            filter.CreationDateFrom = startDate;
//            filter.CreationDateTo = endDate;
//            var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Value.Id.Should().Be(agechange.Id);
//            result.Value.Name.Should().Be(agechange.Fullname.Value);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
//        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
//        {
//            //Act
//            filter.CreationDateFrom = startDate;
//            filter.CreationDateTo = endDate;
//            var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted);
//        }
//    }
//}