using Domain.Entity;

namespace Application.Entity.PatientCards.Queries;

public sealed record PatientCardResponse(Guid Id, byte Age, string Adress, string Complaints, string PhoneNumber)
{
	internal PatientCardResponse(PatientCard ageChange) : this(ageChange.Id, ageChange.Age, ageChange.Adress.Value, ageChange.Complaints.Value, ageChange.PhoneNumber.Value)
	{ }
}
