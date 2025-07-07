using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Users.Queries.GetById;

public sealed record UserGetQuery(EntityQueries<User> Query) : IQuery<UserResponse>;