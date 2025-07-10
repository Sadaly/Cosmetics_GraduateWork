using Application.Abstractions.Messaging;

namespace Application.Entity.SkinFeatureTypes.Commands.Update;
public sealed record SkinFeatureTypeUpdateCommand(Guid SkinFeatureTypeId, string Title) : ICommand<Guid>;