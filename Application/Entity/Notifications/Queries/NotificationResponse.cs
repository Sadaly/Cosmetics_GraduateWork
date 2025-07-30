using Domain.Entity;

namespace Application.Entity.Notifications.Queries;

public sealed record NotificationResponse(Guid Id, Guid ProcedureId, string Message, string PhoneNumber)
{
    internal NotificationResponse(Notification notification) : this(notification.Id, notification.ProcedureId, notification.Message.Value, notification.PhoneNumber.Value)
    { }
}
