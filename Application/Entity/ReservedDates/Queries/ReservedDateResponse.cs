using Domain.Entity;
using Domain.Enums;

namespace Application.Entity.ReservedDates.Queries;

public sealed record ReservedDateResponse(DateTime StartDate, DateTime EndDate, ReservedDateEnumType Type)
{
	internal ReservedDateResponse(ReservedDate reservedDate) : this(reservedDate.StartDate, reservedDate.EndDate, reservedDate.Type)
	{ }
}
