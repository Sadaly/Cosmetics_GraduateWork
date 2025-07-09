using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.SkinFeatures.Queries.Get;
public sealed record SkinFeatureGetQuery(EntityQueries<SkinFeature> Query) : IQuery<SkinFeatureResponse>;
