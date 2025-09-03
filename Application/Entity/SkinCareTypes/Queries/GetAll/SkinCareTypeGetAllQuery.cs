using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.SkinCareTypes.Queries.GetAll;
public sealed record SkinCareTypeGetAllQuery(
	EntityQueries<SkinCareType> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<SkinCareTypeResponse>>;