using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Users.Queries.Get;

public sealed record UserGetQuery(EntityQueries<User> Query) : IQuery<UserResponse>;