using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Queries.GetUserById;

public sealed record UserGetByIdQuery(Guid UserId) : IQuery<UserResponse>;