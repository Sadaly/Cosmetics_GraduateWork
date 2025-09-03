using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Commands.Update;

public sealed record UserUpdateCommand(
	Guid Id,
	string? Username,
	string? Email,
	string? Password) : ICommand<string>;