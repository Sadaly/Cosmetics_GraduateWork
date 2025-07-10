using Application.Abstractions.Messaging;

namespace Application.Entity.Patients.Commands.Update;
public sealed record PatientUpdateCommand(Guid Id,
    string FullName) : ICommand<Guid>;
