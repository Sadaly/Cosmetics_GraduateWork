using Application.Abstractions.Messaging;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.Users.Queries.UsersTake;
public sealed record UsersTakeQuery(
    int StartIndex,
    int Count,
    Expression<Func<User, bool>>? Predicate) : IQuery<List<UserResponse>>;