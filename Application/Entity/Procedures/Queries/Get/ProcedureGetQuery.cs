using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Procedures.Queries.Get;
public sealed record ProcedureGetQuery(EntityQueries<Procedure> Query) : IQuery<ProcedureResponse>;
