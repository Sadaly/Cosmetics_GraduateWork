using Application.Abstractions.Messaging;

namespace Application.Entity.AgeChanges.Commands.Create;
public sealed record AgeChangeCreateCommand(Guid PatientCardId, Guid TypeId) : ICommand<Guid>;
