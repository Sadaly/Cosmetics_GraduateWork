using Domain.Entity;

namespace Application.Entity.Patients.Queries
{
    public sealed record PatientResponses(
    Guid PatientId,
    Guid CardtId,
    string Fullname)
    {
        internal PatientResponses(Patient patient)
            : this(patient.Id, patient.Card.Id, patient.Fullname.Value)
        { }
    }
}
