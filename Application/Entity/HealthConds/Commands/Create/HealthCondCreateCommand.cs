using Application.Abstractions.Messaging;

namespace Application.Entity.HealthConds.Commands.Create;
public sealed record HealthCondCreateCommand(Guid PatientCardId, Guid TypeId) : ICommand<Guid>;
