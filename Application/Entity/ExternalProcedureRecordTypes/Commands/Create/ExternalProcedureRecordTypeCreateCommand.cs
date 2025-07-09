using Application.Abstractions.Messaging;

namespace Application.Entity.ExternalProcedureRecordTypes.Commands.Create;
public sealed record ExternalProcedureRecordTypeCreateCommand(string Title) : ICommand<Guid>;