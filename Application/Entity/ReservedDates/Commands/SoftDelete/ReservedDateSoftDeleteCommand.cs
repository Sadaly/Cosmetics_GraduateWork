using Application.Abstractions.Messaging;

namespace Application.Entity.ReservedDates.Commands.SoftDelete;
public sealed record ReservedDateSoftDeleteCommand(Guid Id) : ICommand<Guid>;