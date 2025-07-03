using Application.Abstractions.Messaging;

namespace Application.Entity.SkinFeatures.Commands.Create;
public sealed record SkinFeatureCreateCommand(Guid PatientCardId, Guid TypeId) : ICommand<Guid>;