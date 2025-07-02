using Application.Abstractions.Messaging;

namespace Application.Entity.Patients.Commands.PatientCreateCommand;
public sealed record PatientCreateCommand(
    string FullName) : ICommand<Guid>;
