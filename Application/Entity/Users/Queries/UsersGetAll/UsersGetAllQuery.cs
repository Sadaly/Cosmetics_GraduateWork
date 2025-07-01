using System.Linq.Expressions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Users.Queries.UsersGetAll;

public sealed record UsersGetAllQuery(
    Expression<Func<User, bool>>? Predicate) : IQuery<List<UserResponse>>;