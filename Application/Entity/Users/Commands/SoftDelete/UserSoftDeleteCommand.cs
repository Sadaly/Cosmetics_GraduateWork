using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Commands.SoftDelete;

public sealed record UserSoftDeleteCommand(
    Guid Id) : ICommand<Guid>;