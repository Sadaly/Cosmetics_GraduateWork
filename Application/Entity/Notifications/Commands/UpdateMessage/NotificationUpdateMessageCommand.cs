using Application.Abstractions.Messaging;

namespace Application.Entity.Notifications.Commands.UpdateMessage;
public sealed record NotificationUpdateMessageCommand(Guid NotificationId, string Message) : ICommand<Guid>;