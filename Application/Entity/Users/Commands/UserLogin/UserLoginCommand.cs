using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Commands.UserLogin;

public sealed record UserLoginCommand(
    string Email, 
    string Password) : ICommand<string>;