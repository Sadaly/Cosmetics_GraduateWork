using Domain.Entity;

namespace Application.Entity.SkinCares.Queries;

public sealed record SkinCareResponse(Guid PatientCardId, Guid TypeId)
{
	internal SkinCareResponse(SkinCare skinCare) : this(skinCare.PatientCardId, skinCare.TypeId)
	{ }
}
