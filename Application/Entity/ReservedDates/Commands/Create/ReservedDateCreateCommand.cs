using Application.Abstractions.Messaging;
using Domain.Enums;

namespace Application.Entity.ReservedDates.Commands.Create;
public sealed record ReservedDateCreateCommand(DateTime StartDate, DateTime EndDate, ReservedDateType Type) : ICommand<Guid>;
