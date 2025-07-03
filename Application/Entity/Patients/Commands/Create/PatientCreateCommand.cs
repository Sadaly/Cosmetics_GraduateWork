using Application.Abstractions.Messaging;

namespace Application.Entity.Patients.Commands.Create;
public sealed record PatientCreateCommand(
    string FullName) : ICommand<Guid>;
