using Application.Abstractions.Messaging;

namespace Application.Entity.ExternalProcedureRecords.Commands.SoftDelete;
public sealed record ExternalProcedureRecordSoftDeleteCommand(Guid Id) : ICommand<Guid>;