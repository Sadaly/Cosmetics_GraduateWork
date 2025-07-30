using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Commands.Login;

public sealed record UserLoginCommand(
    string Email,
    string Password) : ICommand<string>;