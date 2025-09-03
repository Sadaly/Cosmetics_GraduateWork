using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.AgeChanges.Queries.GetAll;
public sealed record AgeChangeGetAllQuery(
	EntityQueries<AgeChange> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<AgeChangeResponse>>;