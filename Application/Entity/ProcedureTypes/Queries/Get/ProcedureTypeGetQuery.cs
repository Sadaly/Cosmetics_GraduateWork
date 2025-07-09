using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.ProcedureTypes.Queries.Get;
public sealed record ProcedureTypeGetQuery(EntityQueries<ProcedureType> Query) : IQuery<ProcedureTypeResponse>;

