using Application.Abstractions.Messaging;

namespace Application.Entity.SkinFeatureTypes.Commands.Create;
public sealed record SkinFeatureTypeCreateCommand(string Title) : ICommand<Guid>;