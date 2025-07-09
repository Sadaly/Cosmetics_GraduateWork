using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.SkinFeatureTypes.Queries;

public sealed record SkinFeatureTypeQueries(Expression<Func<SkinFeatureType, bool>> Predicate) : EntityQueries<SkinFeatureType>(Predicate)
{
}
