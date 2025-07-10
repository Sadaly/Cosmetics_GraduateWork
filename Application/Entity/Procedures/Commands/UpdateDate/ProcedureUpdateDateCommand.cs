using Application.Abstractions.Messaging;

namespace Application.Entity.Procedures.Commands.UpdateDate;
public sealed record ProcedureUpdateDateCommand(Guid ProcedureId, DateTime ScheduledDate, int Duration) : ICommand<Guid>;
