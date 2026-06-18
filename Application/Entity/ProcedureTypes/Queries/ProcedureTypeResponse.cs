using Domain.Entity;

namespace Application.Entity.ProcedureTypes.Queries;

public sealed record ProcedureTypeResponse(Guid Id, string Title, string Description, int Price, int Duration)
{
	internal ProcedureTypeResponse(ProcedureType procedure) : this(procedure.Id, procedure.Title.Value, procedure.StandartDescription ?? "", procedure.StandartPrice, procedure.StandartDuration)
	{ }
}
