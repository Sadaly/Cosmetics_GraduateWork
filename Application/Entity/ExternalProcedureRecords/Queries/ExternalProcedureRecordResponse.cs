using Domain.Entity;

namespace Application.Entity.ExternalProcedureRecords.Queries;

public sealed record ExternalProcedureRecordResponse(Guid PatientCardId, Guid TypeId, string Date, string TypeName = "undentified")
{
	internal ExternalProcedureRecordResponse(ExternalProcedureRecord externalProcedureRecord) : this(externalProcedureRecord.PatientCardId, externalProcedureRecord.TypeId, externalProcedureRecord.Date, externalProcedureRecord.Type.Title.Value)
	{ }
}
