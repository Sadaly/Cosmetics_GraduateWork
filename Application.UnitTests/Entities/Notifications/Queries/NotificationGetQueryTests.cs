using Application.Entity.Notifications.Queries;
using Application.Entity.Notifications.Queries.Get;
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
	public class NotificationGetQueryTests : TestsTheoryData
	{
		private readonly NotificationGetQueryHandler _handler;
		private readonly INotificationRepository _repository;
		private readonly Notification _notification;
		private readonly NotificationFilter _filter;
		private readonly Procedure _procedure;

		public NotificationGetQueryTests()
		{
			_repository = Substitute.For<INotificationRepository>();
			_procedure = Procedure.Create(
				Patient.Create(Username.Create("Fullname")).Value.Card,
				ProcedureType.Create(Title.Create("Create"), "", 10),
				10,
				DateTime.UtcNow.AddDays(6)).Value;
			_notification = Notification.Create(_procedure, Text.Create(""), DateTime.UtcNow.AddMinutes(200), PhoneNumber.Create("")).Value;
			_filter = new NotificationFilter();
			_handler = new NotificationGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<Notification, bool>>>(expr =>
				expr.Compile()(_notification) == true), Arg.Any<CancellationToken>())
				.Returns(_notification);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<Notification, bool>>>(expr =>
				expr.Compile()(_notification) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<Notification>(PersistenceErrors.Entity<Notification>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new NotificationGetQuery(NotificationQueries.GetById(_notification.Id)), default);

			//Assert
			result.Value.Id.Should().Be(_notification.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new NotificationGetQuery(NotificationQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Notification>.NotFound);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new NotificationGetQuery(NotificationQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_notification.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new NotificationGetQuery(NotificationQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Notification>.NotFound);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new NotificationGetQuery(NotificationQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_notification.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new NotificationGetQuery(NotificationQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Notification>.NotFound);
		}
	}
}