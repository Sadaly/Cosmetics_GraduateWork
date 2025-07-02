using Domain.Entity;

namespace Application.Entity.Patients.Queries
{
    public sealed record PatientResponses(
    Guid UserId,
    string Fullname)
    {
        internal PatientResponses(Patient patient)
            : this(patient.Id, patient.Fullname.Value)
        { }
    }
}
