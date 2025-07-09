using Application.Abstractions.Messaging;

namespace Application.Entity.AgeChangeTypes.Commands.Update;
public sealed record AgeChangeTypeUpdateCommand(Guid AgeChangeTypeId, string Title) : ICommand<Guid>;