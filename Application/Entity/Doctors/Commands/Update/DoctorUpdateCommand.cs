using Application.Abstractions.Messaging;

namespace Application.Entity.Doctors.Commands.Update;
public sealed record DoctorUpdateCommand(Guid DoctorId, string Name) : ICommand<Guid>;