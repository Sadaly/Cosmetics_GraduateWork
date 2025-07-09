using Application.Abstractions.Messaging;

namespace Application.Entity.AgeChangeTypes.Commands.SoftDelete;
public sealed record AgeChangeTypeSoftDeleteCommand(Guid Id) : ICommand<Guid>;