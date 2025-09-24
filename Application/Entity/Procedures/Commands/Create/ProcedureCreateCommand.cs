using Application.Abstractions.Messaging;

namespace Application.Entity.Procedures.Commands.Create;
public sealed record ProcedureCreateCommand(Guid PatientCardId, Guid TypeId, int Duration = 0, int Price = 0, Guid? DoctorId = null, DateTime? ScheduledDate = null) : ICommand<Guid>;
