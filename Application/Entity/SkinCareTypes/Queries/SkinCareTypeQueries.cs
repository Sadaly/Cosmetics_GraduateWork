using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.SkinCareTypes.Queries;

public sealed record SkinCareTypeQueries(Expression<Func<SkinCareType, bool>> Predicate) : EntityQueries<SkinCareType>(Predicate)
{
}
