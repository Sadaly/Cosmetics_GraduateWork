using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Queries.UserGetById;

public sealed record UserGetByIdQuery(Guid UserId) : IQuery<UserResponse>;