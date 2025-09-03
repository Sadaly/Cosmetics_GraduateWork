using Domain.Entity;

namespace Application.Entity.Patients.Queries
{
	public sealed record PatientResponse(
	Guid PatientId,
	Guid CardtId,
	string Fullname)
	{
		internal PatientResponse(Patient patient)
			: this(patient.Id, patient.Card.Id, patient.Fullname.Value)
		{ }
	}
}
