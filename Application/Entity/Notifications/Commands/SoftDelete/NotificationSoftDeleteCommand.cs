using Application.Abstractions.Messaging;

namespace Application.Entity.Notifications.Commands.SoftDelete;
public sealed record NotificationSoftDeleteCommand(Guid Id) : ICommand<Guid>;