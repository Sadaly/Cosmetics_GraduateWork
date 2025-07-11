using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.ExternalProcedureRecords.Queries;

public sealed record ExternalProcedureRecordQueries(Expression<Func<ExternalProcedureRecord, bool>> Predicate) : EntityQueries<ExternalProcedureRecord>(Predicate)
{
}
