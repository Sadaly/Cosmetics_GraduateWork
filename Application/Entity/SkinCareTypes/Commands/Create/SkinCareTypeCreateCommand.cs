using Application.Abstractions.Messaging;

namespace Application.Entity.SkinCareTypes.Commands.Create;
public sealed record SkinCareTypeCreateCommand(string Title) : ICommand<Guid>;