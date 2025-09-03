using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.ProcedureTypes.Queries.GetAll;
public sealed record ProcedureTypeGetAllQuery(
	EntityQueries<ProcedureType> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<ProcedureTypeResponse>>;