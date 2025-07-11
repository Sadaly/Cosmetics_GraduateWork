using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.HealthConds.Queries;

public sealed record HealthCondQueries(Expression<Func<HealthCond, bool>> Predicate) : EntityQueries<HealthCond>(Predicate)
{
}
