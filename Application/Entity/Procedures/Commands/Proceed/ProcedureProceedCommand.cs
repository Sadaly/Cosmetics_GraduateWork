using Application.Abstractions.Messaging;

namespace Application.Entity.Procedures.Commands.Proceed;
public sealed record ProcedureProceedCommand(Guid ProcedureId) : ICommand<Guid>;
