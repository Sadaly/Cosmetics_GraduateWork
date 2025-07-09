using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.Procedures.Queries;

public sealed record ProcedureQueries(Expression<Func<SkinFeature, bool>> Predicate) : EntityQueries<SkinFeature>(Predicate)
{
}
