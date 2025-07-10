using Application.Abstractions.Messaging;

namespace Application.Entity.SkinFeatures.Commands.ChangeType;
public sealed record SkinFeatureChangeTypeCommand(Guid SkinFeatureId, Guid TypeId) : ICommand<Guid>;
