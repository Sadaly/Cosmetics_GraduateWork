using Application.Abstractions.Messaging;

namespace Application.Entity.Procedures.Commands.RemoveDoctor;
public sealed record ProcedureRemoveDoctorCommand(Guid ProcedureId) : ICommand<Guid>;
