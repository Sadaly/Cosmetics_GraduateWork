using Application.Abstractions.Messaging;

namespace Application.Entity.Procedures.Commands.Cancel;
public sealed record ProcedureCancelCommand(Guid ProcedureId) : ICommand<Guid>;
