using Domain.Entity;

namespace Application.Entity.ExternalProcedureRecords.Queries;

public sealed record ExternalProcedureRecordResponse(Guid PatientCardId, Guid TypeId, DateOnly? Date)
{
    internal ExternalProcedureRecordResponse(ExternalProcedureRecord externalProcedureRecord) : this(externalProcedureRecord.PatientCardId, externalProcedureRecord.TypeId, externalProcedureRecord.DateOnly) 
    { }
}
