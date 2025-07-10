using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Notifications.Commands.Create
{
    internal class NotificationCreateCommandHandler(INotificationRepository notificationRepository, IProcedureRepository procedureRepository, IUnitOfWork unitOfWork) : ICommandHandler<NotificationCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(NotificationCreateCommand request, CancellationToken cancellationToken)
        {
            var procedure = await procedureRepository.GetByIdAsync(request.ProcedureId, cancellationToken, FetchMode.Include);
            if (procedure.IsFailure) return Result.Failure<Guid>(procedure.Error);

            var text = Text.Create(request.Message);
            var notification = Notification.Create(procedure, text, request.SendDate, procedure.Value.PatientCard.PhoneNumber);

            var add = await notificationRepository.AddAsync(notification, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
