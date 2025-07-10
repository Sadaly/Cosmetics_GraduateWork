using Application.Abstractions.Messaging;

namespace Application.Entity.ExternalProcedureRecords.Commands.ChangeType;
public sealed record ExternalProcedureRecordChangeTypeCommand(Guid ExternalProcedureRecordId, Guid TypeId) : ICommand<Guid>;
