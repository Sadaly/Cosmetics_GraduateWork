using Application.Abstractions.Messaging;

namespace Application.Entity.ExternalProcedureRecordTypes.Commands.Update;
public sealed record ExternalProcedureRecordTypeUpdateCommand(Guid ExternalProcedureRecordTypeId, string Title) : ICommand<Guid>;