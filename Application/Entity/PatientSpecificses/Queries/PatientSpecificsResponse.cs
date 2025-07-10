using Domain.Entity;

namespace Application.Entity.PatientSpecificses.Queries;

public sealed record PatientSpecificsResponse(Guid PatientCardId, string Sleep, string Diet, string Sport, string WorkEnviroment)
{
    internal PatientSpecificsResponse(PatientSpecifics patientSpecifics) : this(patientSpecifics.PatientCardId, patientSpecifics.Sleep, patientSpecifics.Diet, patientSpecifics.Sport, patientSpecifics.WorkEnviroment) 
    { }
}
