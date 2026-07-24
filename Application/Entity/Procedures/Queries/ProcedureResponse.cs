using Domain.Entity;

namespace Application.Entity.Procedures.Queries;

public sealed record ProcedureResponse(Guid ProcedureId, Guid PatientCardId, int Price, DateTime? ScheduledDate, Guid TypeId, string Title, Guid? DoctorId,
		bool IsComplete, bool IsCancelled)
{
	internal ProcedureResponse(Procedure procedure) : this(procedure.Id, procedure.PatientCardId, procedure.Price, procedure.ScheduledDate, procedure.TypeId, procedure.Type.Title.Value, procedure.DoctorId,
		procedure.IsComplete, procedure.IsCancelled)
	{ }
}
