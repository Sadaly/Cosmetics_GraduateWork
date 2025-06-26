using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Commands.UserCreate;

public sealed record UserCreateCommand(
    string Username,
    string Email,
    string Password) : ICommand<Guid>;