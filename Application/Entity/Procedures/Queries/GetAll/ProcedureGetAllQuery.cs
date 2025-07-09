using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Procedures.Queries.GetAll;
public sealed record ProcedureGetAllQuery(
    EntityQueries<Procedure> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<ProcedureResponse>>;