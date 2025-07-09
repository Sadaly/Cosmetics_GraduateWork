using Domain.Entity;

namespace Application.Entity.AgeChanges.Queries;

public sealed record AgeChangeResponse(Guid PatientCardId, Guid TypeId)
{
    internal AgeChangeResponse(AgeChange ageChange) : this(ageChange.PatientCardId, ageChange.TypeId) 
    { }
}
