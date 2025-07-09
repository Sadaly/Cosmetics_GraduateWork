using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.ExternalProcedureRecordTypes.Queries.GetAll;
public sealed record ExternalProcedureRecordTypeGetAllQuery(
    EntityQueries<ExternalProcedureRecordType> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<ExternalProcedureRecordTypeResponse>>;