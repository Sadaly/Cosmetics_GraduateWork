using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Notifications.Commands.SoftDelete
{
    internal class NotificationSoftDeleteCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : ICommandHandler<NotificationSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(NotificationSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await notificationRepository.GetByIdAsync(request.Id, cancellationToken);
            var remove = await notificationRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
