using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.HealthCondTypes.Queries;

public sealed record HealthCondTypeQueries(Expression<Func<HealthCondType, bool>> Predicate) : EntityQueries<HealthCondType>(Predicate)
{
}
