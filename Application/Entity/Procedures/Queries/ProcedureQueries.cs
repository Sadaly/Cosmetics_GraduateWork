using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.Procedures.Queries;

public sealed record ProcedureQueries(Expression<Func<Procedure, bool>> Predicate) : EntityQueries<Procedure>(Predicate)
{
}
