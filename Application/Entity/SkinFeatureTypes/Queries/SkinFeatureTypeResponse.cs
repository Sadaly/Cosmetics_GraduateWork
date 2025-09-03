using Domain.Entity;

namespace Application.Entity.SkinFeatureTypes.Queries;

public sealed record SkinFeatureTypeResponse(Guid Id, string Title)
{
	internal SkinFeatureTypeResponse(SkinFeatureType skinFeature) : this(skinFeature.Id, skinFeature.Title.Value)
	{ }
}
