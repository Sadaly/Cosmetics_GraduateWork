using Domain.Entity;

namespace Application.Entity.SkinFeatures.Queries;

public sealed record SkinFeatureResponse(Guid PatientCardId, Guid TypeId)
{
    internal SkinFeatureResponse(SkinFeature SkinFeature) : this(SkinFeature.PatientCardId, SkinFeature.TypeId) 
    { }
}
