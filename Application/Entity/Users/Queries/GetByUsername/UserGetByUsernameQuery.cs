using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Queries.UserGetByUsername;

public sealed record UserGetByUsernameQuery(string Username) : IQuery<UserResponse>;