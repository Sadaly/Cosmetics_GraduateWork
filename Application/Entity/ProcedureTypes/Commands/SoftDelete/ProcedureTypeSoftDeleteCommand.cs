using Application.Abstractions.Messaging;

namespace Application.Entity.ProcedureTypes.Commands.SoftDelete;
public sealed record ProcedureTypeSoftDeleteCommand(Guid Id) : ICommand<Guid>;