using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.ProcedureTypes.Queries;

public sealed record ProcedureTypeQueries(Expression<Func<ProcedureType, bool>> Predicate) : EntityQueries<ProcedureType>(Predicate)
{
}
