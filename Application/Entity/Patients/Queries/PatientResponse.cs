using Domain.Entity;

namespace Application.Entity.Patients.Queries
{
	public sealed record PatientResponse(
	Guid PatientId,
	string Fullname,
	Guid CardId,
	string Address,
	int Age,
	string Complaints,
	string PhoneNumber)
	{
		internal PatientResponse(Patient patient)
			: this(patient.Id, patient.Fullname.Value, patient.Card.Id, patient.Card.Address.Value, patient.Card.Age, patient.Card.Complaints.Value, patient.Card.PhoneNumber.Value)
		{ }
	}
}
