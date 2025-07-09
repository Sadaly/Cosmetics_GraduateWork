using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.AgeChanges.Queries;

public sealed record AgeChangeQueries(Expression<Func<AgeChange, bool>> Predicate) : EntityQueries<AgeChange>(Predicate)
{
}
