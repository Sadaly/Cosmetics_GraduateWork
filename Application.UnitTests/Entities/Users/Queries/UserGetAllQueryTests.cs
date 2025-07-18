//using Application.Entity.Users.Queries;
//using Application.Entity.Users.Queries.GetAll;
//using Application.UnitTests.TheoryData;
//using Domain.Entity;
//using Domain.Repositories;
//using Domain.SupportData.Filters;
//using Domain.ValueObjects;
//using FluentAssertions;
//using NSubstitute;
//using System.Linq.Expressions;

//namespace Application.UnitTests.Entities.Users.Queries
//{
//    public class UserGetAllQueryTests : TestsTheoryData
//    {
//        private readonly UserGetAllQueryHandler _handler;
//        private readonly IUserRepository _repository;
//        private readonly User _user1;
//        private readonly User _user2;
//        private readonly UserFilter _filter;

//        public UserGetAllQueryTests()
//        {
//            _repository = Substitute.For<IUserRepository>();
//            _user1 = User.Create(Username.Create("Fullname1").Value).Value;
//            _user2 = User.Create(Username.Create("Fullname2").Value).Value;
//            _filter = new UserFilter();
//            _handler = new UserGetAllQueryHandler(_repository);

//            _repository.GetAllAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
//                expr.Compile()(_user1) == true && expr.Compile()(_user2) == true), Arg.Any<CancellationToken>())
//                .Returns(new List<User>(){ _user1, _user2 });

//            _repository.GetAllAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
//                expr.Compile()(_user1) == true && expr.Compile()(_user2) == false), Arg.Any<CancellationToken>())
//                .Returns(new List<User>(){ _user1 });

//            _repository.GetAllAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
//                expr.Compile()(_user2) == true && expr.Compile()(_user1) == false), Arg.Any<CancellationToken>())
//                .Returns(new List<User>(){ _user2 });
            
//            _repository.GetAllAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
//                expr.Compile()(_user1) == false && expr.Compile()(_user2) == false), Arg.Any<CancellationToken>())
//                .Returns(new List<User>(){});
//        }

//        [Fact]
//        public async Task Handle_Should_ReturnSuccess_WhenValidId()
//        {
//            //Act
//            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetWithoutPredicate()), default);

//            //Assert
//            result.Value[0].Id.Should().Be(_user1.Id);
//            result.Value[1].Id.Should().Be(_user2.Id);
//        }

//        [Theory]
//        [MemberData(nameof(ValidNameGetTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
//        {
//            //Act
//            _filter.Name = name;
//            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

//            //Assert
//            result.Value[0].Id.Should().Be(_user1.Id);
//            result.Value[1].Id.Should().Be(_user2.Id);
//        }

//        [Theory]
//        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
//        {
//            //Act
//            _filter.Name = name;
//            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

//            //Assert
//            result.Value.Count.Should().Be(1);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidNameGetTestCases))]
//        public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
//        {
//            //Act
//            _filter.Name = name;
//            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

//            //Assert
//            result.Value.Count.Should().Be(0);
//        }
//        [Theory]
//        [MemberData(nameof(ValidCreationDatesGetTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
//        {
//            //Act
//            _filter.CreationDateFrom = startDate;
//            _filter.CreationDateTo = endDate;
//            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

//            //Assert
//            result.Value[0].Id.Should().Be(_user1.Id);
//            result.Value[1].Id.Should().Be(_user2.Id);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
//        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
//        {
//            //Act
//            _filter.CreationDateFrom = startDate;
//            _filter.CreationDateTo = endDate;
//            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

//            //Assert
//            result.Value.Count.Should().Be(0);
//        }

//        [Theory]
//        [MemberData(nameof(ValidIndexesGetAllTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
//        {
//            //Arrange
//            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())
//                .Returns(new List<User>() { _user1, _user2 }.Skip(startIndex).Take(count).ToList());
            
//            //Act
//            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetWithoutPredicate(), startIndex, count), default);

//            //Assert
//            result.Value.Count.Should().Be(count - startIndex);
//        }
//    }
//}