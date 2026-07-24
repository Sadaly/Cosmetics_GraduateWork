using Domain.Entity;

namespace Application.Entity.SkinCares.Queries;

public sealed record SkinCareResponse(Guid PatientCardId, Guid TypeId, string TypeName = "undentified")
{
	internal SkinCareResponse(SkinCare skinCare) : this(skinCare.PatientCardId, skinCare.TypeId, skinCare.Type.Title.Value)
	{ }
}
