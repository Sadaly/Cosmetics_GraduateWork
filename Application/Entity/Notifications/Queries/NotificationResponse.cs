using Domain.Entity;

namespace Application.Entity.Notifications.Queries;

public sealed record NotificationResponse(Guid Id, string Message, string PhoneNumber)
{
    internal NotificationResponse(Notification ageChange) : this(ageChange.Id, ageChange.Message.Value, ageChange.PhoneNumber.Value) 
    { }
}
