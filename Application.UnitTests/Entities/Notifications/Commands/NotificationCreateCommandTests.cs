using Application.Entity.Notifications.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Notifications.Commands
{
	public class NotificationCreateCommandTests : TestsTheoryData
	{
		private readonly NotificationCreateCommandHandler _handler;
		private readonly INotificationRepository _repository;
		private readonly IProcedureRepository _procedureRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly Procedure _procedure;

		public NotificationCreateCommandTests()
		{
			_repository = Substitute.For<INotificationRepository>();
			_procedureRepository = Substitute.For<IProcedureRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();

			_handler = new NotificationCreateCommandHandler(_repository, _procedureRepository, _unitOfWork);

			_procedure = Procedure.Create(
				Patient.Create(Username.Create("Fullname")).Value.Card,
				ProcedureType.Create(Title.Create("Create"), "", 10, 10),
				10,
				10,
				DateTime.UtcNow.AddDays(6)).Value;

			_repository.AddAsync(Arg.Any<Result<Notification>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<Notification>>());

			_procedureRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _procedure.Id), Arg.Any<CancellationToken>(), FetchMode.Include)
				.Returns(_procedure);

			_procedureRepository.GetByIdAsync(Arg.Is<Guid>(x => x != _procedure.Id), Arg.Any<CancellationToken>(), FetchMode.Include)
				.Returns(Result.Failure<Procedure>(PersistenceErrors.Entity<Procedure>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<Notification>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<Notification>>());
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidInput()
		{
			if (_procedure.ScheduledDate == null)
				return;
			//Act
			var result = await _handler.Handle(new NotificationCreateCommand(_procedure.Id, "", _procedure.ScheduledDate.Value.AddMinutes(-1)), default);

			//Assert
			result.IsSuccess.Should().BeTrue();
		}
		[Fact]
		public async Task Handle_Should_ReturnError_WhenInvalidInput()
		{
			if (_procedure.ScheduledDate == null)
				return;
			//Act
			var result = await _handler.Handle(new NotificationCreateCommand(Guid.NewGuid(), "", _procedure.ScheduledDate.Value.AddMinutes(-1)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Procedure>.NotFound);
		}
		[Fact]
		public async Task Handle_Should_ReturnError_WhenSaveIsFailer()
		{
			if (_procedure.ScheduledDate == null)
				return;
			//Arrange
			_unitOfWork.SaveChangesAsync(Arg.Any<Result<Notification>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<Notification>(PersistenceErrors.Entity<Notification>.NotFound));

			//Act
			var result = await _handler.Handle(new NotificationCreateCommand(_procedure.Id, "", _procedure.ScheduledDate.Value.AddMinutes(-1)), default);

			//Assert
			result.IsFailure.Should().Be(true);
		}
	}
}