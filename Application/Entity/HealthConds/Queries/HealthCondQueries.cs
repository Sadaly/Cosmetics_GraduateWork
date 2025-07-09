using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.HealthConds.Queries;

public sealed record HealthCondQueries(Expression<Func<SkinFeature, bool>> Predicate) : EntityQueries<SkinFeature>(Predicate)
{
}
