using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Notifications.Commands.UpdateMessage
{
    internal class NotificationUpdateMessageCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : ICommandHandler<NotificationUpdateMessageCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(NotificationUpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var message = Text.Create(request.Message);
            var notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
            if (notification.IsFailure) return Result.Failure<Guid>(notification.Error);

            var update = notification.Value.UpdateMessage(message);
            var add = await notificationRepository.UpdateAsync(update, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
