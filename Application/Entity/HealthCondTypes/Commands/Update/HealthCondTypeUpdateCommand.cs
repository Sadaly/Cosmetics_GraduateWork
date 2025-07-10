using Application.Abstractions.Messaging;

namespace Application.Entity.HealthCondTypes.Commands.Update;
public sealed record HealthCondTypeUpdateCommand(Guid HealthCondTypeId, string Title) : ICommand<Guid>;