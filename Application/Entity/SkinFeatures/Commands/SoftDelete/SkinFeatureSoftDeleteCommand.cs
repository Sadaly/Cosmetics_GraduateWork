using Application.Abstractions.Messaging;

namespace Application.Entity.SkinFeatures.Commands.SoftDelete;
public sealed record SkinFeatureSoftDeleteCommand(Guid Id) : ICommand<Guid>;