using Domain.Entity;

namespace Application.Entity.HealthConds.Queries;

public sealed record HealthCondResponse(Guid PatientCardId, Guid TypeId, string TypeName = "undentified")
{
	internal HealthCondResponse(HealthCond healthCond) : this(healthCond.PatientCardId, healthCond.TypeId, healthCond.Type.Title.Value)
	{ }
}
