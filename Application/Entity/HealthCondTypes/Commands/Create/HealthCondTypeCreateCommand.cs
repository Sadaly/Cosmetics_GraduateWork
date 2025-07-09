using Application.Abstractions.Messaging;

namespace Application.Entity.HealthCondTypes.Commands.Create;
public sealed record HealthCondTypeCreateCommand(string Title) : ICommand<Guid>;