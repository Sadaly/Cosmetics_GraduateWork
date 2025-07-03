using Application.Abstractions.Messaging;

namespace Application.Entity.SkinFeatureTypes.Commands.SoftDelete;
public sealed record SkinFeatureTypeSoftDeleteCommand(Guid Id) : ICommand<Guid>;