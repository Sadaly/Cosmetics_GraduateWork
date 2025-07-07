using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Users.Queries.GetAll;

public sealed record UsersGetAllQuery(
    EntityQueries<User> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<UserResponse>>;