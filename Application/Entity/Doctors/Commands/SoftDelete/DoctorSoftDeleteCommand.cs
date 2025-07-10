using Application.Abstractions.Messaging;

namespace Application.Entity.Doctors.Commands.SoftDelete;
public sealed record DoctorSoftDeleteCommand(Guid Id) : ICommand<Guid>;