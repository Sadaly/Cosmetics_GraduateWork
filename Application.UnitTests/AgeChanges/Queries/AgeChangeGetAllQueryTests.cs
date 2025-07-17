//using Application.Entity.AgeChanges.Queries;
//using Application.Entity.AgeChanges.Queries.GetAll;
//using Application.UnitTests.TheoryData;
//using Domain.Entity;
//using Domain.Repositories;
//using Domain.SupportData.Filters;
//using Domain.ValueObjects;
//using FluentAssertions;
//using NSubstitute;
//using System.Linq.Expressions;

//namespace Application.UnitTests.AgeChanges.Queries
//{
//    public class AgeChangeGetAllQueryTests : TestsTheoryData
//    {
//        private readonly AgeChangeGetAllQueryHandler _handler;
//        private readonly IAgeChangeRepository _repository;
//        private readonly AgeChange agechange1 = AgeChange.Create(Username.Create("Fullname1").Value).Value;
//        private readonly AgeChange agechange2 = AgeChange.Create(Username.Create("Fullname2").Value).Value;
//        private readonly AgeChangeFilter filter = new AgeChangeFilter();

//        public AgeChangeGetAllQueryTests()
//        {
//            _repository = Substitute.For<IAgeChangeRepository>();

//            _handler = new AgeChangeGetAllQueryHandler(_repository);

//            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
//                expr.Compile()(agechange1) == true && expr.Compile()(agechange2) == true), Arg.Any<CancellationToken>())
//                .Returns(new List<AgeChange>(){ agechange1, agechange2 });

//            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
//                expr.Compile()(agechange1) == true && expr.Compile()(agechange2) == false), Arg.Any<CancellationToken>())
//                .Returns(new List<AgeChange>(){ agechange1 });

//            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
//                expr.Compile()(agechange2) == true && expr.Compile()(agechange1) == false), Arg.Any<CancellationToken>())
//                .Returns(new List<AgeChange>(){ agechange2 });
            
//            _repository.GetAllAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
//                expr.Compile()(agechange1) == false && expr.Compile()(agechange2) == false), Arg.Any<CancellationToken>())
//                .Returns(new List<AgeChange>(){});
//        }

//        [Fact]
//        public async Task Handle_Should_ReturnSuccess_WhenValidId()
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetWithoutPredicate()), default);

//            //Assert
//            result.Value[0].Id.Should().Be(agechange1.Id);
//            result.Value[1].Id.Should().Be(agechange2.Id);
//        }

//        [Theory]
//        [MemberData(nameof(ValidNameGetTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
//        {
//            //Act
//            filter.Name = name;
//            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Value[0].Id.Should().Be(agechange1.Id);
//            result.Value[1].Id.Should().Be(agechange2.Id);
//        }

//        [Theory]
//        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
//        {
//            //Act
//            filter.Name = name;
//            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Value.Count.Should().Be(1);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidNameGetTestCases))]
//        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
//        {
//            //Act
//            filter.Name = name;
//            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Value.Count.Should().Be(0);
//        }
//        [Theory]
//        [MemberData(nameof(ValidCreationDatesGetTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
//        {
//            //Act
//            filter.CreationDateFrom = startDate;
//            filter.CreationDateTo = endDate;
//            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Value[0].Id.Should().Be(agechange1.Id);
//            result.Value[1].Id.Should().Be(agechange2.Id);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
//        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
//        {
//            //Act
//            filter.CreationDateFrom = startDate;
//            filter.CreationDateTo = endDate;
//            var result = await _handler.Handle(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(filter)), default);

//            //Assert
//            result.Value.Count.Should().Be(0);
//        }
//    }
//}