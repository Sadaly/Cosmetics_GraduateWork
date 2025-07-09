using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.ExternalProcedureRecords.Queries.GetAll;
public sealed record ExternalProcedureRecordGetAllQuery(
    EntityQueries<ExternalProcedureRecord> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<ExternalProcedureRecordResponse>>;