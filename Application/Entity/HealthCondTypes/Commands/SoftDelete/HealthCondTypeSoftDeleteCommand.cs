using Application.Abstractions.Messaging;

namespace Application.Entity.HealthCondTypes.Commands.SoftDelete;
public sealed record HealthCondTypeSoftDeleteCommand(Guid Id) : ICommand<Guid>;