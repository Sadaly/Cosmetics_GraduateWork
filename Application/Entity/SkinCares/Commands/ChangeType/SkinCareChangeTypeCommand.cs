using Application.Abstractions.Messaging;

namespace Application.Entity.SkinCares.Commands.ChangeType;
public sealed record SkinCareChangeTypeCommand(Guid SkinCareId, Guid TypeId) : ICommand<Guid>;
