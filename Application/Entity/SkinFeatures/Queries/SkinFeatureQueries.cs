using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.SkinFeatures.Queries;

public sealed record SkinFeatureQueries(Expression<Func<SkinFeature, bool>> Predicate) : EntityQueries<SkinFeature>(Predicate)
{
}
