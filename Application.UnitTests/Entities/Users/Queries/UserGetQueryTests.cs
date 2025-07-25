using Application.Entity.Users.Queries;
using Application.Entity.Users.Queries.Get;
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

namespace Application.UnitTests.Entities.Users.Queries
{
    public class UserGetQueryTests : TestsTheoryData
    {
        private readonly UserGetQueryHandler _handler;
        private readonly IUserRepository _repository;
        private readonly User _user;
        private readonly Username _username;
        private readonly Email _email;
        private readonly PasswordHashed _password;
        private readonly UserFilter _filter;

        public UserGetQueryTests()
        {
            _repository = Substitute.For<IUserRepository>();

            _username = Username.Create("Fullname").Value;
            _email = Email.Create("str@ing").Value;
            _password = PasswordHashed.Create("password").Value;
            _user = User.Create(_email, _username, _password).Value;

            _filter = new UserFilter();
            _handler = new UserGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
                expr.Compile()(_user) == true), Arg.Any<CancellationToken>())
                .Returns(_user);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<User, bool>>>(expr =>
                expr.Compile()(_user) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<User>(PersistenceErrors.Entity<User>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new UserGetQuery(UserQueries.GetById(_user.Id)), default);

            //Assert
            result.Value.UserId.Should().Be(_user.Id);
            result.Value.Username.Should().Be(_user.Username.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new UserGetQuery(UserQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<User>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Username = name;
            var result = await _handler.Handle(new UserGetQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.UserId.Should().Be(_user.Id);
            result.Value.Username.Should().Be(_user.Username.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.Username = name;
            var result = await _handler.Handle(new UserGetQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<User>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.RegistrationDateFrom = startDate;
            _filter.RegistrationDateTo = endDate;
            var result = await _handler.Handle(new UserGetQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.UserId.Should().Be(_user.Id);
            result.Value.Username.Should().Be(_user.Username.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.RegistrationDateFrom = startDate;
            _filter.RegistrationDateTo = endDate;
            var result = await _handler.Handle(new UserGetQuery(UserQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<User>.NotFound);
        }
    }
}