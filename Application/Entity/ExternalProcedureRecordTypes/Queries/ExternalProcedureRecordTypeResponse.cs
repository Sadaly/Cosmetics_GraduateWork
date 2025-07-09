using Domain.Entity;

namespace Application.Entity.ExternalProcedureRecordTypes.Queries;

public sealed record ExternalProcedureRecordTypeResponse(Guid Id, string Title)
{
    internal ExternalProcedureRecordTypeResponse(ExternalProcedureRecordType externalProcedureRecord) : this(externalProcedureRecord.Id, externalProcedureRecord.Title.Value) 
    { }
}
