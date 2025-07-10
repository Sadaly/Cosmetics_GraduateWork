using Application.Abstractions.Messaging;

namespace Application.Entity.SkinCareTypes.Commands.Update;
public sealed record SkinCareTypeUpdateCommand(Guid SkinCareTypeId, string Title) : ICommand<Guid>;