using Application.Abstractions;
using Application.Entity.Users.Queries;
using Application.Entity.Users.Queries.GetAll;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.SupportData.Filters;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.Users.Queries
{
    public class UserGetAllQueryTests : TestsTheoryData
    {
        private readonly UserGetAllQueryHandler _handler;
        private readonly IUserRepository _repository;
        private readonly User _user1;
        private readonly Username _username1;
        private readonly Email _email1;
        private readonly PasswordHashed _password1;
        private readonly User _user2;
        private readonly Username _username2;
        private readonly Email _email2;
        private readonly PasswordHashed _password2;
        private readonly UserFilter _filter;

        public UserGetAllQueryTests()
        {
            _repository = Substitute.For<IUserRepository>();

            _username1 = Username.Create("Fullname1").Value;
            _email1 = Email.Create("str@ing").Value;
            _password1 = PasswordHashed.Create("password").Value;
            _user1 = User.Create(_email1, _username1, _password1).Value;

            _username2 = Username.Create("Fullname2").Value;
            _email2 = Email.Create("str@ing").Value;
            _password2 = PasswordHashed.Create("password").Value;
            _user2 = User.Create(_email2, _username2, _password2).Value;

            _filter = new UserFilter();
            _handler = new UserGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
                expr.Compile()(_user1) == true && expr.Compile()(_user2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<User>() { _user1, _user2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
                expr.Compile()(_user1) == true && expr.Compile()(_user2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<User>() { _user1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
                expr.Compile()(_user2) == true && expr.Compile()(_user1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<User>() { _user2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
                expr.Compile()(_user1) == false && expr.Compile()(_user2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<User>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].UserId.Should().Be(_user1.Id);
            result.Value[1].UserId.Should().Be(_user2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Username = name;
            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].UserId.Should().Be(_user1.Id);
            result.Value[1].UserId.Should().Be(_user2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
        {
            //Act
            _filter.Username = name;
            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
        {
            //Act
            _filter.Username = name;
            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.RegistrationDateFrom = startDate;
            _filter.RegistrationDateTo = endDate;
            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].UserId.Should().Be(_user1.Id);
            result.Value[1].UserId.Should().Be(_user2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.RegistrationDateFrom = startDate;
            _filter.RegistrationDateTo = endDate;
            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<User>() { _user1, _user2 }.Skip(startIndex).Take(count).ToList());

            //Act
            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<User>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<User>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<User>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new UserGetAllQuery(UserQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}