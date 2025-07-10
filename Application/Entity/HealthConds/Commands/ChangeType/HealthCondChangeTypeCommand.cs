using Application.Abstractions.Messaging;

namespace Application.Entity.HealthConds.Commands.ChangeType;
public sealed record HealthCondChangeTypeCommand(Guid HealthCondId, Guid TypeId) : ICommand<Guid>;
