using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.ExternalProcedureRecords.Queries.Get;
public sealed record ExternalProcedureRecordGetQuery(EntityQueries<ExternalProcedureRecord> Query) : IQuery<ExternalProcedureRecordResponse>;
