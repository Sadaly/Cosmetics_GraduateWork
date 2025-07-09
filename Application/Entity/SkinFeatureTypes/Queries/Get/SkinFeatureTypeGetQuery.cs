using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.SkinFeatureTypes.Queries.Get;
public sealed record SkinFeatureTypeGetQuery(EntityQueries<SkinFeatureType> Query) : IQuery<SkinFeatureTypeResponse>;

