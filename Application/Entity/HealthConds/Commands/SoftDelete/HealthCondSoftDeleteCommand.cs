using Application.Abstractions.Messaging;

namespace Application.Entity.HealthConds.Commands.SoftDelete;
public sealed record HealthCondSoftDeleteCommand(Guid Id) : ICommand<Guid>;