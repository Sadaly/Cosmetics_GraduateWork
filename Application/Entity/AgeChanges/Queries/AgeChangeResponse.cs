using Domain.Entity;

namespace Application.Entity.AgeChanges.Queries;

public sealed record AgeChangeResponse(Guid PatientCardId, Guid TypeId, string TypeName = "undentified")
{
	internal AgeChangeResponse(AgeChange ageChange) : this(ageChange.PatientCardId, ageChange.TypeId, ageChange.Type.Title.Value)
	{ }
}
