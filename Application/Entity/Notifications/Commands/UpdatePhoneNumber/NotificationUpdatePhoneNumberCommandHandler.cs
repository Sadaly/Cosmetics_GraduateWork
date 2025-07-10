using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Notifications.Commands.UpdatePhoneNumber
{
    internal class NotificationUpdateCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : ICommandHandler<NotificationUpdatePhoneNumberCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(NotificationUpdatePhoneNumberCommand request, CancellationToken cancellationToken)
        {
            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
            var notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
            if (notification.IsFailure) return Result.Failure<Guid>(notification.Error);

            var update = notification.Value.UpdatePhoneNumber(phoneNumber);
            var add = await notificationRepository.UpdateAsync(update, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
