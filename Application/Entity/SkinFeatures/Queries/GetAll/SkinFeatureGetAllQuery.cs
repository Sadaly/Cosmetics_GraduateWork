using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.SkinFeatures.Queries.GetAll;
public sealed record SkinFeatureGetAllQuery(
    EntityQueries<SkinFeature> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<SkinFeatureResponse>>;