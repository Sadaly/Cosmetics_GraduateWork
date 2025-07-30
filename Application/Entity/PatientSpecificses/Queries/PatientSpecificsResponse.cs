using Domain.Entity;

namespace Application.Entity.PatientSpecificses.Queries;

public sealed record PatientSpecificsResponse(Guid Id, Guid PatientCardId, string Sleep, string Diet, string Sport, string WorkEnviroment)
{
    internal PatientSpecificsResponse(PatientSpecifics patientSpecifics) : this(patientSpecifics.Id, patientSpecifics.PatientCardId, patientSpecifics.Sleep, patientSpecifics.Diet, patientSpecifics.Sport, patientSpecifics.WorkEnviroment)
    { }
}
