using Domain.Entity;

namespace Application.Entity.SkinCareTypes.Queries;

public sealed record SkinCareTypeResponse(Guid Id, string Title)
{
	internal SkinCareTypeResponse(SkinCareType skinCare) : this(skinCare.Id, skinCare.Title.Value)
	{ }
}
