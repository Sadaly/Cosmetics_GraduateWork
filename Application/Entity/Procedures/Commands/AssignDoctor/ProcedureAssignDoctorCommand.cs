using Application.Abstractions.Messaging;

namespace Application.Entity.Procedures.Commands.AssignDoctor;
public sealed record ProcedureAssignDoctorCommand(Guid ProcedureId, Guid DoctorId) : ICommand<Guid>;
