using Application.Abstractions.Messaging;

namespace Application.Entity.Notifications.Commands.Create;
public sealed record NotificationCreateCommand(Guid ProcedureId, string Message, DateTime SendDate) : ICommand<Guid>;