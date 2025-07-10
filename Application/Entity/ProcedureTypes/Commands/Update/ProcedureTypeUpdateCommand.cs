using Application.Abstractions.Messaging;

namespace Application.Entity.ProcedureTypes.Commands.Update;
public sealed record ProcedureTypeUpdateCommand(Guid ProcedureTypeId, string Title) : ICommand<Guid>;