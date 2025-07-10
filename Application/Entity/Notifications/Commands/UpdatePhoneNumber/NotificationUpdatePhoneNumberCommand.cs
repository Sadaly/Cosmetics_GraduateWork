using Application.Abstractions.Messaging;

namespace Application.Entity.Notifications.Commands.UpdatePhoneNumber;
public sealed record NotificationUpdatePhoneNumberCommand(Guid NotificationId, string PhoneNumber) : ICommand<Guid>;