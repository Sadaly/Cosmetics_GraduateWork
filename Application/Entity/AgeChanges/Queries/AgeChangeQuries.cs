using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.AgeChanges.Queries;

public sealed record AgeChangeQuries(Expression<Func<AgeChange, bool>> Predicate) : EntityQueries<AgeChange>(Predicate)
{
}
