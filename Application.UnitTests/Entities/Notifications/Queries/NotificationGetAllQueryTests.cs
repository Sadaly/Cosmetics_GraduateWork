using Application.Entity.Notifications.Queries;
using Application.Entity.Notifications.Queries.GetAll;
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

namespace Application.UnitTests.Entities.Notifications.Queries
{
    public class NotificationGetAllQueryTests : TestsTheoryData
    {
        private readonly NotificationGetAllQueryHandler _handler;
        private readonly INotificationRepository _repository;
        private readonly Notification _notification1;
        private readonly Notification _notification2;
        private readonly NotificationFilter _filter;
        private readonly Procedure _procedure;

        public NotificationGetAllQueryTests()
        {
            _repository = Substitute.For<INotificationRepository>();
            _procedure = Procedure.Create(
                Patient.Create(Username.Create("Fullname1")).Value.Card,
                ProcedureType.Create(Title.Create("Create"), "", 10),
                10,
                DateTime.UtcNow.AddDays(6)).Value;
            _notification1 = Notification.Create(_procedure, Text.Create(""), DateTime.UtcNow.AddMinutes(200), PhoneNumber.Create("")).Value;

            _procedure = Procedure.Create(
                Patient.Create(Username.Create("Fullname2")).Value.Card,
                ProcedureType.Create(Title.Create("Create"), "", 10),
                10,
                DateTime.UtcNow.AddDays(6)).Value;
            _notification2 = Notification.Create(_procedure, Text.Create(""), DateTime.UtcNow.AddMinutes(200), PhoneNumber.Create("")).Value;

            _filter = new NotificationFilter();
            _handler = new NotificationGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<Notification, bool>>>(expr =>
                expr.Compile()(_notification1) == true && expr.Compile()(_notification2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<Notification>() { _notification1, _notification2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Notification, bool>>>(expr =>
                expr.Compile()(_notification1) == true && expr.Compile()(_notification2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Notification>() { _notification1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Notification, bool>>>(expr =>
                expr.Compile()(_notification2) == true && expr.Compile()(_notification1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Notification>() { _notification2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Notification, bool>>>(expr =>
                expr.Compile()(_notification1) == false && expr.Compile()(_notification2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Notification>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new NotificationGetAllQuery(NotificationQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].Id.Should().Be(_notification1.Id);
            result.Value[1].Id.Should().Be(_notification2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new NotificationGetAllQuery(NotificationQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_notification1.Id);
            result.Value[1].Id.Should().Be(_notification2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new NotificationGetAllQuery(NotificationQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
        {
            //Act
            _filter.PatientName = name;
            var result = await _handler.Handle(new NotificationGetAllQuery(NotificationQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new NotificationGetAllQuery(NotificationQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_notification1.Id);
            result.Value[1].Id.Should().Be(_notification2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new NotificationGetAllQuery(NotificationQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<Notification, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Notification>() { _notification1, _notification2 }.Skip(startIndex).Take(count).ToList());

            //Act
            var result = await _handler.Handle(new NotificationGetAllQuery(NotificationQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<Notification, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Notification>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<Notification, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Notification>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<Notification, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Notification>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new NotificationGetAllQuery(NotificationQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}