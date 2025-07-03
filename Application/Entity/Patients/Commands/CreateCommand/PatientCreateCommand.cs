using Application.Abstractions.Messaging;

namespace Application.Entity.Patients.Commands.CreateCommand;
public sealed record PatientCreateCommand(
    string FullName) : ICommand<Guid>;
