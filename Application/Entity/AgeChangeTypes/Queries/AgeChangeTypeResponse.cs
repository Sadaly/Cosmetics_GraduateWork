using Domain.Entity;

namespace Application.Entity.AgeChangeTypes.Queries;

public sealed record AgeChangeTypeResponse(Guid Id, string Title)
{
    internal AgeChangeTypeResponse(AgeChangeType ageChange) : this(ageChange.Id, ageChange.Title.Value)
    { }
}
