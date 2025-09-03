using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.AgeChangeTypes.Queries.GetAll;
public sealed record AgeChangeTypeGetAllQuery(
	EntityQueries<AgeChangeType> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<AgeChangeTypeResponse>>;