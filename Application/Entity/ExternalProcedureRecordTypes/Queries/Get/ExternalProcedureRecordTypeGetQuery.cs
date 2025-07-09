using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.ExternalProcedureRecordTypes.Queries.Get;
public sealed record ExternalProcedureRecordTypeGetQuery(EntityQueries<ExternalProcedureRecordType> Query) : IQuery<ExternalProcedureRecordTypeResponse>;

