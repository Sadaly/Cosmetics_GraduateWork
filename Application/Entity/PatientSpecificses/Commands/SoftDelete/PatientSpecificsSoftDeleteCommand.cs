using Application.Abstractions.Messaging;

namespace Application.Entity.PatientSpecificses.Commands.SoftDelete;
public sealed record PatientSpecificsSoftDeleteCommand(Guid Id) : ICommand<Guid>;