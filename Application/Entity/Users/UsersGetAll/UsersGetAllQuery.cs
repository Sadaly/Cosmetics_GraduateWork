using System.Linq.Expressions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Users.Queries.GetUserById;

public sealed record UsersGetAllQuery(
    Expression<Func<User, bool>>? Predicate) : IQuery<List<UserResponse>>;