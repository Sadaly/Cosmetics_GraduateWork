using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.ReservedDates.Queries;

public sealed record ReservedDateQueries(Expression<Func<ReservedDate, bool>> Predicate) : EntityQueries<ReservedDate>(Predicate)
{
}
