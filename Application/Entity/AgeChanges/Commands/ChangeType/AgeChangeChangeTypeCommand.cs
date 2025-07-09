using Application.Abstractions.Messaging;

namespace Application.Entity.AgeChanges.Commands.ChangeType;
public sealed record AgeChangeChangeTypeCommand(Guid AgeChangeId, Guid TypeId) : ICommand<Guid>;
