using Domain.Entity;

namespace Application.Entity.ProcedureTypes.Queries;

public sealed record ProcedureTypeResponse(Guid Id, string Title, int Price, int Duration)
{
	internal ProcedureTypeResponse(ProcedureType procedure) : this(procedure.Id, procedure.Title.Value, procedure.StandartPrice, procedure.StandartDuration)
	{ }
}
