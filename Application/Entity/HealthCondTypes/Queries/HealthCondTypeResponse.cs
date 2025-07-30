using Domain.Entity;

namespace Application.Entity.HealthCondTypes.Queries;

public sealed record HealthCondTypeResponse(Guid Id, string Title)
{
    internal HealthCondTypeResponse(HealthCondType healthCond) : this(healthCond.Id, healthCond.Title.Value)
    { }
}
