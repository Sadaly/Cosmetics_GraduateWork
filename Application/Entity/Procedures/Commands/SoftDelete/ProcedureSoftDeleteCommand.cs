using Application.Abstractions.Messaging;

namespace Application.Entity.Procedures.Commands.SoftDelete;
public sealed record ProcedureSoftDeleteCommand(Guid Id) : ICommand<Guid>;