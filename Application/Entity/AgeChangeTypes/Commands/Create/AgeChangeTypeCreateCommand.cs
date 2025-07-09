using Application.Abstractions.Messaging;

namespace Application.Entity.AgeChangeTypes.Commands.Create;
public sealed record AgeChangeTypeCreateCommand(string Title) : ICommand<Guid>;