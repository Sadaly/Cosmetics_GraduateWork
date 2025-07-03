using Application.Abstractions.Messaging;

namespace Application.Entity.Patients.Commands.SoftDelete;
public sealed record PatientSoftDeleteCommand(Guid PatientId) : ICommand<Guid>;
