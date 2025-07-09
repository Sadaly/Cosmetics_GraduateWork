using Domain.Entity;

namespace Application.Entity.ProcedureTypes.Queries;

public sealed record ProcedureTypeResponse(Guid Id, string Title)
{
    internal ProcedureTypeResponse(ProcedureType procedure) : this(procedure.Id, procedure.Title.Value) 
    { }
}
