using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.ExternalProcedureRecordTypes.Queries;

public sealed record ExternalProcedureRecordTypeQueries(Expression<Func<ExternalProcedureRecordType, bool>> Predicate) : EntityQueries<ExternalProcedureRecordType>(Predicate)
{
}
