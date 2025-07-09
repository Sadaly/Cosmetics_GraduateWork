using Application.Abstractions.Messaging;

namespace Application.Entity.AgeChanges.Commands.SoftDelete;
public sealed record AgeChangeSoftDeleteCommand(Guid Id) : ICommand<Guid>;