using Application.Abstractions.Messaging;

namespace Application.Entity.Users.Queries.GetById;

public sealed record UserGetByIdQuery(Guid UserId) : IQuery<UserResponse>;