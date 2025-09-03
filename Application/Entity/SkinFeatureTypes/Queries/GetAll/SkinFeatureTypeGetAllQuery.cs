using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.SkinFeatureTypes.Queries.GetAll;
public sealed record SkinFeatureTypeGetAllQuery(
	EntityQueries<SkinFeatureType> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<SkinFeatureTypeResponse>>;