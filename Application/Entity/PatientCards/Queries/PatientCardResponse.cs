using Domain.Entity;

namespace Application.Entity.PatientCards.Queries;

public sealed record PatientCardResponse(Guid Id, Guid PatientId, string Fullname, byte Age, string Address, string Complaints, string PhoneNumber)
{
	internal PatientCardResponse(PatientCard patientCard) : this(patientCard.Id, patientCard.PatientId, patientCard.Patient.Fullname.Value, patientCard.Age, patientCard.Address.Value, patientCard.Complaints.Value, patientCard.PhoneNumber.Value)
	{ }
}
