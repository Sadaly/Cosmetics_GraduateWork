using Domain.Entity;

namespace Application.Entity.SkinFeatureTypes.Queries;

public sealed record SkinFeatureTypeResponse(Guid Id, string Title)
{
    internal SkinFeatureTypeResponse(SkinFeatureType SkinFeature) : this(SkinFeature.Id, SkinFeature.Title.Value) 
    { }
}
