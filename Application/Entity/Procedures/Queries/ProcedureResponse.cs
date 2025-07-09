using Domain.Entity;

namespace Application.Entity.Procedures.Queries;

public sealed record ProcedureResponse(Guid PatientCardId, Guid TypeId)
{
    internal ProcedureResponse(Procedure procedure) : this(procedure.PatientCardId, procedure.TypeId) 
    { }
}
