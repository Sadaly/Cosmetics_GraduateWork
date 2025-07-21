using Application.Entity.Notifications.Commands.UpdateMessage;
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
    public class NotificationUpdateMessageCommandTests : TestsTheoryData
    {
        private readonly NotificationUpdateMessageCommandHandler _handler;
        private readonly INotificationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Notification _notification;
        private readonly Procedure _procedure;

        public NotificationUpdateMessageCommandTests()
        {
            _repository = Substitute.For<INotificationRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _procedure = Procedure.Create(
                Patient.Create(Username.Create("Fullname")).Value.Card,
                ProcedureType.Create(Title.Create("Create"), "", 10),
                10,
                DateTime.UtcNow.AddDays(6)).Value;
            _notification = Notification.Create(_procedure, Text.Create(""), DateTime.UtcNow.AddMinutes(200), PhoneNumber.Create("")).Value;

            _handler = new NotificationUpdateMessageCommandHandler(_repository, _unitOfWork);
            _repository.UpdateAsync(Arg.Any<Result<Notification>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Notification>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _notification.Id), Arg.Any<CancellationToken>())
                .Returns(_notification);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _notification.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<Notification>(PersistenceErrors.Entity<Notification>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<Notification>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<Notification>>());
        }


        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new NotificationUpdateMessageCommand(_notification.Id, ""), default);

            //Assert
            result.Value.Should().Be(_notification.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new NotificationUpdateMessageCommand(Guid.Parse(id), ""), default);

            result.Error.Should().Be(PersistenceErrors.Entity<Notification>.NotFound);
        }
    }
}