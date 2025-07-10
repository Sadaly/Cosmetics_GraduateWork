using Application.Abstractions.Messaging;

namespace Application.Entity.Doctors.Commands.Create;
public sealed record DoctorCreateCommand(string Name) : ICommand<Guid>;