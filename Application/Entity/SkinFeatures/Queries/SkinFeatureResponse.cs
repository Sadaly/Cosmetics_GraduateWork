using Domain.Entity;

namespace Application.Entity.SkinFeatures.Queries;

public sealed record SkinFeatureResponse(Guid PatientCardId, Guid TypeId, string TypeName = "undentified")
{
	internal SkinFeatureResponse(SkinFeature skinFeature) : this(skinFeature.PatientCardId, skinFeature.TypeId, skinFeature.Type.Title.Value)
	{ }
}
