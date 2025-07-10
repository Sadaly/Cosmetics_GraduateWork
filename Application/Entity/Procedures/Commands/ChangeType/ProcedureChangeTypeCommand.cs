using Application.Abstractions.Messaging;

namespace Application.Entity.Procedures.Commands.ChangeType;
public sealed record ProcedureChangeTypeCommand(Guid ProcedureId, Guid TypeId) : ICommand<Guid>;
