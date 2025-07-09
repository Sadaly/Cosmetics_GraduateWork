using Application.Abstractions.Messaging;

namespace Application.Entity.SkinCares.Commands.Create;
public sealed record SkinCareCreateCommand(Guid PatientCardId, Guid TypeId) : ICommand<Guid>;
