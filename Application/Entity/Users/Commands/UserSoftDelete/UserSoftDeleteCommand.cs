using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Commands.UserCreate;

public sealed record UserSoftDeleteCommand(
    Guid Id) : ICommand<Guid>;