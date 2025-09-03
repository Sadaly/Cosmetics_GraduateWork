using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Commands.Create;

public sealed record UserCreateCommand(
	string Username,
	string Email,
	string Password) : ICommand<Guid>;