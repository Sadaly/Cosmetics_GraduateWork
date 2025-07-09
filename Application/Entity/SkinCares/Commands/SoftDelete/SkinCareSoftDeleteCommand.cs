using Application.Abstractions.Messaging;

namespace Application.Entity.SkinCares.Commands.SoftDelete;
public sealed record SkinCareSoftDeleteCommand(Guid Id) : ICommand<Guid>;