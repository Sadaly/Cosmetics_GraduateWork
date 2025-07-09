using Application.Abstractions.Messaging;

namespace Application.Entity.ExternalProcedureRecords.Commands.Create;
public sealed record ExternalProcedureRecordCreateCommand(Guid PatientCardId, Guid TypeId, DateOnly? Date) : ICommand<Guid>;
