using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.AgeChangeTypes.Queries;

public sealed record AgeChangeTypeQueries(Expression<Func<AgeChangeType, bool>> Predicate) : EntityQueries<AgeChangeType>(Predicate)
{
}
