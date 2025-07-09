using Application.Abstractions.Messaging;

namespace Application.Entity.ExternalProcedureRecordTypes.Commands.SoftDelete;
public sealed record ExternalProcedureRecordTypeSoftDeleteCommand(Guid Id) : ICommand<Guid>;