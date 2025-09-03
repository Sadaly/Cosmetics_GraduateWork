using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.HealthConds.Queries.GetAll;
public sealed record HealthCondGetAllQuery(
	EntityQueries<HealthCond> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<HealthCondResponse>>;