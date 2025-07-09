using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.SkinCares.Queries;

public sealed record SkinCareQueries(Expression<Func<SkinFeature, bool>> Predicate) : EntityQueries<SkinFeature>(Predicate)
{
}
